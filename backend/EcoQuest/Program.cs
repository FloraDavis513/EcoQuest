using Microsoft.EntityFrameworkCore;

namespace EcoQuest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            string dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<eco_questContext>(options => options.UseNpgsql(dbConnectionString));
            builder.Services.AddCors();

            WebApplication app = builder.Build();

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });










            app.MapGet("/admin/leaders", (eco_questContext db) =>
            {
                Console.WriteLine("==========/admin/leaders==========");
            });

            app.MapGet("/admin/waiting", (eco_questContext db) =>
            {
                Console.WriteLine("==========/admin/waiting==========");
            });

            app.MapPost("/auth/login", (eco_questContext db) =>
            {
                Console.WriteLine("==========/auth/login==========");
            });

            app.MapPost("/auth/register", (eco_questContext db) =>
            {
                Console.WriteLine("==========/auth/register==========");
            });
            
            








            //Создание нового шаблона
            app.MapPost("/board/create", (eco_questContext db, BoardCreate newBoardCreate) =>
            {
                Console.WriteLine("==========/board/create==========");

                GameBoard newGameBoard = new GameBoard() { Name = newBoardCreate.Name, NumFields = newBoardCreate.NumFields };
                db.GameBoards.Add(newGameBoard);
                db.SaveChanges();

                GameBoard addedGameBoard = (from gb in db.GameBoards where gb.Name == newGameBoard.Name select gb).ToList().First();
                long addedGameBoardId = addedGameBoard.Id;

                foreach (var item in newBoardCreate.ProductWithQuestionRqs)
                {
                    foreach (var questionId in item.QuestionIds)
                    {
                        db.GameBoardsQuestions.Add(new GameBoardsQuestion() { GameBoardId = addedGameBoardId, QuestionId = questionId });
                    }
                }

                foreach (var item in newBoardCreate.ProductWithQuestionRqs)
                {
                    db.ProductsForBoards.Add(new ProductsForBoard() { GameBoardId = addedGameBoardId, ProductId = item.ProductId, NumOfRepeating = item.NumberOfRepeating });
                }

                db.SaveChanges();
            });

            //Удаление существующего шаблона по id
            app.MapDelete("/board/delete/{id:long}", (eco_questContext db, long id) =>
            {
                Console.WriteLine("==========/board/delete/{id:long}==========");

                GameBoard? targetGameBoard = (from gb in db.GameBoards where gb.Id == id select gb).ToArray().FirstOrDefault();
                if (targetGameBoard != null)
                {
                    db.GameBoards.Remove(targetGameBoard);
                }

                List<GameBoardsQuestion> targetGameBoardQuestions = (from q in db.GameBoardsQuestions where q.GameBoardId == id select q).ToList();
                if (targetGameBoardQuestions.Count > 0)
                {
                    db.GameBoardsQuestions.RemoveRange(targetGameBoardQuestions);
                }

                List<ProductsForBoard> targetProductsForBoard = (from p in db.ProductsForBoards where p.GameBoardId == id select p).ToList();
                if (targetProductsForBoard.Count > 0)
                {
                    db.ProductsForBoards.RemoveRange(targetProductsForBoard);
                }

                db.SaveChanges();
            });

            //Получение существующего шаблона по id
            app.MapGet("/board/get/{id:long}", (eco_questContext db, long id) =>
            {
                Console.WriteLine("==========/board/get/{id:long}==========");

                GameBoard targetGameBoard = (from gb in db.GameBoards where gb.Id == id select gb).ToArray().First();

                BoardGet boardGet = new BoardGet();

                boardGet.Id = targetGameBoard.Id;
                boardGet.Name = targetGameBoard.Name;
                boardGet.NumFields = targetGameBoard.NumFields;

                List<ProductsForBoard> targetProductsForBoard = (from p in db.ProductsForBoards where p.GameBoardId == id select p).ToList();

                foreach (var product in targetProductsForBoard)
                {
                    Product? targetProductForBoard = (from p in db.Products where p.Id == product.ProductId select p).ToArray().FirstOrDefault();

                    if (targetProductForBoard != null)
                    {
                        List<Question> allQuestions = db.Questions.ToList();
                        List<Question> targetGameBoardsQuestions = new List<Question>();

                        List<Question> targetProductQuestoins = (from q in db.Questions where q.ProductId == product.ProductId select q).ToList();
                        List<long> targetGameBoardsQuestionIds = (from q in db.GameBoardsQuestions where q.GameBoardId == id select q.QuestionId).ToList();

                        foreach (var targetGameBoardsQuestionId in targetGameBoardsQuestionIds)
                        {
                            Question? question = (from q in allQuestions where q.Id == targetGameBoardsQuestionId select q).ToList().FirstOrDefault();
                            if (question != null)
                            {
                                targetGameBoardsQuestions.Add(question);
                            }
                        }

                        List<Question> targetQuestions = new List<Question>();
                        if (targetGameBoardsQuestions.Count > 0 && targetProductQuestoins.Count > 0)
                        {
                            targetQuestions = targetGameBoardsQuestions.Intersect(targetProductQuestoins).ToList();
                        }

                        ProductGet productGet = new ProductGet() { Id = product.ProductId, Name = targetProductForBoard.Name, Colour = targetProductForBoard.Colour, NumOfRepeating = product.NumOfRepeating };

                        foreach (var question in targetQuestions)
                        {
                            productGet.Questions.Add(question);
                        }

                        boardGet.Products.Add(productGet);
                    }
                }

                return Results.Json(boardGet);
            });

            //Получение всех существующих шаблонов
            app.MapGet("/board/getAll", (eco_questContext db) =>
            {
                Console.WriteLine("==========/board/getAll==========");

                List<GameBoard> allGameBoards = db.GameBoards.ToList();

                return Results.Json(allGameBoards);
            });

            //Обновление данных о существующем шаблоне
            app.MapPost("/board/update", (eco_questContext db, BoardCreate newBoardCreate) =>
            {
                Console.WriteLine("==========/board/update==========");

                GameBoard targetGameBoard = (from gb in db.GameBoards where gb.Id == newBoardCreate.Id select gb).ToList().First();

                targetGameBoard.Name = newBoardCreate.Name;
                targetGameBoard.NumFields = newBoardCreate.NumFields;

                db.SaveChanges();

                List<GameBoardsQuestion> targetGameBoardQuestions = (from q in db.GameBoardsQuestions where q.GameBoardId == newBoardCreate.Id select q).ToList();
                if (targetGameBoardQuestions.Count > 0)
                {
                    db.GameBoardsQuestions.RemoveRange(targetGameBoardQuestions);
                }

                List<long> gameBoardQuestionIds = new List<long>();
                foreach (var productWithQuestion in newBoardCreate.ProductWithQuestionRqs)
                {
                    foreach (var gameBoardQuestionId in productWithQuestion.QuestionIds)
                    {
                        gameBoardQuestionIds.Add(gameBoardQuestionId);
                    }
                }

                List<GameBoardsQuestion> gameBoardQuestions = new List<GameBoardsQuestion>();
                foreach (var questionId in gameBoardQuestionIds)
                {
                    gameBoardQuestions.Add(new GameBoardsQuestion { GameBoardId = newBoardCreate.Id, QuestionId = questionId });
                }

                if (gameBoardQuestions.Count > 0)
                {
                    db.GameBoardsQuestions.AddRange(gameBoardQuestions);
                }

                db.SaveChanges();

                List<ProductsForBoard> targetProductsForBoard = (from p in db.ProductsForBoards where p.GameBoardId == newBoardCreate.Id select p).ToList();
                if (targetProductsForBoard.Count > 0)
                {
                    db.ProductsForBoards.RemoveRange(targetProductsForBoard);
                }

                List<ProductsForBoard> productsForBoards = new List<ProductsForBoard>();
                foreach (var productWithQuestion in newBoardCreate.ProductWithQuestionRqs)
                {
                    productsForBoards.Add(new ProductsForBoard() { GameBoardId = newBoardCreate.Id, ProductId = productWithQuestion.ProductId, NumOfRepeating = productWithQuestion.NumberOfRepeating });
                }

                if (productsForBoards.Count > 0)
                {
                    db.ProductsForBoards.AddRange(productsForBoards);
                }

                db.SaveChanges();
            });

            //Заглушка
            //Установка вопроса
            app.MapPost("/game/chooseQuestion", (eco_questContext db, GameChooseQuestion inputData) =>
            {
                Console.WriteLine("==========/game/chooseQuestion==========");

                Session session = (from s in db.Sessions select s).ToArray().First();
                session.IdCurrentQuestion = inputData.QuestionId;
                db.SaveChanges();
            });

            //Заглушка
            //Получение вопроса и ответа
            app.MapGet("/game/getAnswer", (eco_questContext db) =>
            {
                Console.WriteLine("==========/game/getAnswer==========");

                long? idCurrentQuestion = (from s in db.Sessions select s.IdCurrentQuestion).ToArray().First();
                Question? question = (from q in db.Questions where q.Id == idCurrentQuestion select q).ToArray().FirstOrDefault();

                return Results.Json(new { Answer = question.Answer, Question = question.Text });
            });

            //Создание нового продукта (вместе с вопросами)
            app.MapPost("/product/create", (eco_questContext db, Product newProduct) =>
            {
                Console.WriteLine("==========/product/create==========");
                
                db.Products.Add(newProduct);
                db.SaveChanges();
            });

            //Удаление существующего продукта по id (вместе с вопросами)
            app.MapDelete("/product/delete/{id:long}", (eco_questContext db, long id) =>
            {
                Console.WriteLine("==========/product/delete/{id:long}==========");
                
                List<Product> allProducts = db.Products.ToList();
                List<Question> allQuestions = db.Questions.ToList();
                List<ProductsForBoard> allProductsForBoards = db.ProductsForBoards.ToList();

                var targetProductSearch = (from p in allProducts where p.Id == id select p).ToList();
                var targetQuestionsSearch = (from q in allQuestions where q.ProductId == id select q).ToList();
                var targetProductsForBoards = (from p in allProductsForBoards where p.ProductId == id select p).ToList();

                if (targetProductSearch.Count == 1)
                {
                    Product targetProduct = targetProductSearch[0];
                    db.Products.Remove(targetProduct);
                }
                
                foreach (var question in targetQuestionsSearch)
                {
                    db.Questions.Remove(question);
                }

                foreach (var productForBoards in targetProductsForBoards)
                {
                    db.ProductsForBoards.Remove(productForBoards);
                }

                db.SaveChanges();
            });

            //Получение всех существующих продуктов (вместе с вопросами)
            app.MapGet("/product/getAll/{round:int?}", (eco_questContext db, int? round) =>
            {
                Console.WriteLine("==========/product/getAll/{round:int?}==========");
                
                List<Product> allProducts = db.Products.ToList();
                List<Product> roundProducts = (from p in allProducts where p.Round == round select p).ToList();

                List<Question> allQuestions = db.Questions.ToList();

                foreach (var product in roundProducts)
                {
                    var searchQuestions = from q in allQuestions where q.ProductId == product.Id select q;

                    foreach (var question in searchQuestions)
                    {
                        product.Questions.Add(question);
                    }
                }

                return Results.Json(roundProducts);
            });

            //Удаление существующего вопроса по id
            app.MapDelete("/product/question/{id:long}", (eco_questContext db, long id) =>
            {
                Console.WriteLine("==========/product/question/{id:long}==========");
                
                List<Question> allQuestions = db.Questions.ToList();
                List<GameBoardsQuestion> allGameBoardsQuestions = db.GameBoardsQuestions.ToList();

                var targetQuestionSearch = (from q in allQuestions where q.Id == id select q).ToList();
                var targetGameBoardsQuestionsSearch = (from q in allGameBoardsQuestions where q.QuestionId == id select q).ToList();

                if (targetQuestionSearch.Count == 1)
                {
                    Question targetQuestion = targetQuestionSearch[0];
                    db.Questions.Remove(targetQuestion);
                }

                foreach (var gameBoardsQuestion in targetGameBoardsQuestionsSearch)
                {
                    db.GameBoardsQuestions.Remove(gameBoardsQuestion);
                }

                db.SaveChanges();
            });

            //Обновление данных о существующем продукте (вместе с вопросами)
            app.MapPost("/product/update", (eco_questContext db, Product newProduct) =>
            {
                Console.WriteLine("==========/product/update==========");
                
                List<Product> allProducts = db.Products.ToList();

                var targetProductSearch = (from p in allProducts where p.Id == newProduct.Id select p).ToList();

                if (targetProductSearch.Count == 0)
                {
                    db.Products.Add(newProduct);
                }
                else if (targetProductSearch.Count == 1)
                {
                    Product targetProduct = targetProductSearch[0];
                    targetProduct.Colour = newProduct.Colour;
                    targetProduct.Name = newProduct.Name;

                    foreach (var newQuestion in newProduct.Questions)
                    {
                        var targetQuestionSearch = (from q in targetProduct.Questions where q.Id == newQuestion.Id select q).ToList();

                        if (targetQuestionSearch.Count == 0)
                        {
                            targetProduct.Questions.Add(newQuestion);
                        }
                        else if (targetQuestionSearch.Count == 1)
                        {
                            Question targetQuestion = targetQuestionSearch[0];
                            targetQuestion.Answer = newQuestion.Answer;
                            targetQuestion.Type = newQuestion.Type;
                            targetQuestion.ShortText = newQuestion.ShortText;
                            targetQuestion.Text = newQuestion.Text;
                        }
                    }
                }

                db.SaveChanges();
            });










            ////Получение всех существующих продуктов (версия для тестов)
            app.MapGet("/product/getAll", (eco_questContext db) =>
            {
                Console.WriteLine("==========/product/getAll==========");

                List<Product> allProducts = db.Products.ToList();
                List<Question> allQuestions = db.Questions.ToList();

                foreach (var product in allProducts)
                {
                    var searchQuestions = from q in allQuestions where q.ProductId == product.Id select q;

                    foreach (var question in searchQuestions)
                    {
                        product.Questions.Add(question);
                    }
                }

                return Results.Json(allProducts);
            });










            app.Run();
        }
    }

    public class BoardCreate
    {
        public BoardCreate()
        {
            ProductWithQuestionRqs = new HashSet<ProductWithQuestionRqs>();
        }

        public long Id { get; set; }
        public string? Name { get; set; }
        public virtual ICollection<ProductWithQuestionRqs> ProductWithQuestionRqs { get; set; }
        public int? NumFields { get; set; }
    }

    public class ProductWithQuestionRqs
    {
        public ProductWithQuestionRqs()
        {
            QuestionIds = new HashSet<long>();
        }

        public long ProductId { get; set; }
        public int? NumberOfRepeating { get; set; }
        public virtual ICollection<long> QuestionIds { get; set; }
    }

    public class BoardGet
    {
        public BoardGet()
        {
            Products = new HashSet<ProductGet>();
        }

        public long Id { get; set; }
        public string? Name { get; set; }
        public virtual ICollection<ProductGet> Products { get; set; }
        public int? NumFields { get; set; }
    }

    public class ProductGet
    {
        public ProductGet()
        {
            Questions = new HashSet<Question>();
        }

        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Colour { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public int? NumOfRepeating { get; set; }
    }

    public class GameChooseQuestion
    {
        public long? QuestionId { get; set; }
        public string? QuestionType { get; set; }
        public string? State { get; set; }
    }
}