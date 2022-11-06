using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EcoQuest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            string dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<eco_questContext>(options => options.UseNpgsql(dbConnectionString));
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters { ValidateIssuer = true, ValidIssuer = AuthenticationOptions.ISSUER, ValidateAudience = true,
                    ValidAudience = AuthenticationOptions.AUDIENCE, ValidateLifetime = true, IssuerSigningKey = AuthenticationOptions.GetSymmetricSecurityKey(), ValidateIssuerSigningKey = true };
            });
            builder.Services.AddAuthorization();
            builder.Services.AddCors();

            WebApplication app = builder.Build();

            //app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });
            //builder.Services.Configure<ForwardedHeadersOptions>(options => options.KnownProxies.Add(IPAddress.Parse("213.189.217.150")));

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
            });









            
            app.MapGet("/", [Authorize(Roles = "adminadmin, masteractive, masterinactive, player")] (eco_questContext db) =>
            {
                Console.WriteLine("==========/==========");

                return Results.Ok();
            });

            app.MapGet("/auth/login", [Authorize(Roles = "adminadmin, masteractive, masterinactive, player")] (eco_questContext db) =>
            {
                Console.WriteLine("==========/auth/login==========");

                return Results.Ok();
            });

            app.MapGet("/auth/registration", [Authorize(Roles = "adminadmin, masteractive, masterinactive, player")] (eco_questContext db) =>
            {
                Console.WriteLine("==========/auth/registration==========");

                return Results.Ok();
            });

            app.MapGet("/fields", [Authorize(Roles = "adminadmin")] (eco_questContext db) =>
            {
                Console.WriteLine("==========/fields==========");

                return Results.Ok();
            });

            app.MapGet("/game", [Authorize(Roles = "adminadmin, masteractive, player")] (eco_questContext db) =>
            {
                Console.WriteLine("==========/game==========");

                return Results.Ok();
            });

            app.MapGet("/lobby", [Authorize(Roles = "adminadmin, masteractive, player")] (eco_questContext db) =>
            {
                Console.WriteLine("==========/lobby==========");

                return Results.Ok();
            });

            app.MapGet("/status", [Authorize(Roles = "adminadmin, masteractive")] (eco_questContext db) =>
            {
                Console.WriteLine("==========/status==========");

                return Results.Ok();
            });

            app.MapGet("/templates", [Authorize(Roles = "adminadmin, masteractive")] (eco_questContext db) =>
            {
                Console.WriteLine("==========/templates==========");

                return Results.Ok();
            });
            
            








            //Получение списка подтвержденных ведущих
            app.MapGet("/admin/leaders", (eco_questContext db) =>
            {
                Console.WriteLine("==========/admin/leaders==========");

                List<User> activeMasters = (from u in db.Users where u.Role == "master" && u.Status == "active" select u).ToList();

                List<AdminLeadersWaitingResponse> response = new List<AdminLeadersWaitingResponse>();

                foreach (var activeMaster in activeMasters)
                {
                    response.Add(new AdminLeadersWaitingResponse() { UserId = activeMaster.UserId, LastName = activeMaster.LastName,
                        FirstName = activeMaster.FirstName, Patronymic = activeMaster.Patronymic, Login = activeMaster.Login });
                }

                return Results.Json(response);
            });

            //Подтверждение ведущего по id
            app.MapGet("/admin/register/approve/{userId:long}", (eco_questContext db, long userId) =>
            {
                Console.WriteLine("==========/admin/register/approve/{userId:long}==========");

                User? approvedUser = (from u in db.Users where u.UserId == userId select u).ToList().FirstOrDefault();

                if (approvedUser != null)
                {
                    approvedUser.Status = "active";
                    db.SaveChanges();
                }
            });

            //Удаление ведущего по id
            app.MapGet("/admin/register/decline/{userId}", (eco_questContext db, long userId) =>
            {
                Console.WriteLine("==========/admin/register/decline/{userId:long}==========");

                User? declinedUser = (from u in db.Users where u.UserId == userId select u).ToList().FirstOrDefault();

                if (declinedUser != null)
                {
                    db.Users.Remove(declinedUser);
                    db.SaveChanges();
                }
            });

            //Получение списка ведущих, ожидающих подтверждения
            app.MapGet("/admin/waiting", (eco_questContext db) =>
            {
                Console.WriteLine("==========/admin/waiting==========");

                List<User> inactiveMasters = (from u in db.Users where u.Role == "master" && u.Status == "inactive" select u).ToList();

                List<AdminLeadersWaitingResponse> response = new List<AdminLeadersWaitingResponse>();

                foreach (var inactiveMaster in inactiveMasters)
                {
                    response.Add(new AdminLeadersWaitingResponse() { UserId = inactiveMaster.UserId, LastName = inactiveMaster.LastName,
                        FirstName = inactiveMaster.FirstName, Patronymic = inactiveMaster.Patronymic, Login = inactiveMaster.Login });
                }

                return Results.Json(response);
            });

            //Вход в систему по логину и паролю (для ведущих)
            app.MapPost("/auth/login/master", (eco_questContext db, AuthLoginMasterRequest request) =>
            {
                Console.WriteLine("==========/auth/login/master==========");

                User? user = (from u in db.Users where u.Login == request.Login && u.Password == Encrypt(request.Password) select u).ToList().FirstOrDefault();

                if (user == null)
                {
                    return Results.Unauthorized();
                }

                List<Claim> claims = new List<Claim>() { new Claim(ClaimTypes.Name, user.Login), new Claim(ClaimTypes.Role, $"{user.Role + user.Status}") };
                JwtSecurityToken JWT = new JwtSecurityToken(issuer: AuthenticationOptions.ISSUER, audience: AuthenticationOptions.AUDIENCE, claims: claims, expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                    signingCredentials: new SigningCredentials(AuthenticationOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                string encodedJWT = new JwtSecurityTokenHandler().WriteToken(JWT);

                return Results.Json(new { Login = user.Login, AuthorizationToken = encodedJWT });
            });

            //Вход в систему по id комнаты и логину (для игроков)
            app.MapPost("/auth/login/player", (eco_questContext db, AuthLoginPlayerRequest request) =>
            {
                Console.WriteLine("==========/auth/login/player==========");

                object game = null; // Здесь будет обращение к БД и поиск игры с нужным id

                if (game == null)
                {
                    return Results.Unauthorized();
                }

                List<Claim> claims = new List<Claim>() { new Claim(ClaimTypes.Name, request.Login), new Claim(ClaimTypes.Role, "player") };
                JwtSecurityToken JWT = new JwtSecurityToken(issuer: AuthenticationOptions.ISSUER, audience: AuthenticationOptions.AUDIENCE, claims: claims, expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                    signingCredentials: new SigningCredentials(AuthenticationOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                string encodedJWT = new JwtSecurityTokenHandler().WriteToken(JWT);

                return Results.Json(new { GameId = request.GameId, Login = request.Login, AuthorizationToken = encodedJWT });
            });

            //Добавление нового ведущего
            app.MapPost("/auth/register", (eco_questContext db, User request) =>
            {
                Console.WriteLine("==========/auth/register==========");

                User newUser = new User() { LastName = request.LastName, FirstName = request.FirstName, Patronymic = request.Patronymic,
                    Login = request.Login, Password = Encrypt(request.Password), Role = "master", Status = "inactive" };

                db.Users.Add(newUser);
                db.SaveChanges();
            });

            //Создание нового шаблона
            app.MapPost("/board/create", (eco_questContext db, BoardCreateRequest request) =>
            {
                Console.WriteLine("==========/board/create==========");

                GameBoard newGameBoard = new GameBoard() { Name = request.Name, NumFields = request.NumFields };
                db.GameBoards.Add(newGameBoard);
                db.SaveChanges();

                GameBoard addedGameBoard = (from gb in db.GameBoards where gb.Name == newGameBoard.Name select gb).ToList().First();
                long addedGameBoardId = addedGameBoard.Id;

                foreach (var item in request.ProductWithQuestionRqs)
                {
                    foreach (var questionId in item.QuestionIds)
                    {
                        db.GameBoardsQuestions.Add(new GameBoardsQuestion() { GameBoardId = addedGameBoardId, QuestionId = questionId });
                    }
                }

                foreach (var item in request.ProductWithQuestionRqs)
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

                BoardGetResponse response = new BoardGetResponse();

                response.Id = targetGameBoard.Id;
                response.Name = targetGameBoard.Name;
                response.NumFields = targetGameBoard.NumFields;

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

                        ProductResponse productResponse = new ProductResponse() { Id = product.ProductId, Name = targetProductForBoard.Name,
                            Colour = targetProductForBoard.Colour, NumOfRepeating = product.NumOfRepeating };

                        foreach (var question in targetQuestions)
                        {
                            productResponse.Questions.Add(question);
                        }

                        response.Products.Add(productResponse);
                    }
                }

                return Results.Json(response);
            });

            //Получение всех существующих шаблонов
            app.MapGet("/board/getAll", (eco_questContext db) =>
            {
                Console.WriteLine("==========/board/getAll==========");

                List<GameBoard> allGameBoards = db.GameBoards.ToList();

                return Results.Json(allGameBoards);
            });

            //Обновление данных о существующем шаблоне
            app.MapPost("/board/update", (eco_questContext db, BoardCreateRequest request) =>
            {
                Console.WriteLine("==========/board/update==========");

                GameBoard targetGameBoard = (from gb in db.GameBoards where gb.Id == request.Id select gb).ToList().First();

                targetGameBoard.Name = request.Name;
                targetGameBoard.NumFields = request.NumFields;

                db.SaveChanges();

                List<GameBoardsQuestion> targetGameBoardQuestions = (from q in db.GameBoardsQuestions where q.GameBoardId == request.Id select q).ToList();
                if (targetGameBoardQuestions.Count > 0)
                {
                    db.GameBoardsQuestions.RemoveRange(targetGameBoardQuestions);
                }

                List<long> gameBoardQuestionIds = new List<long>();
                foreach (var productWithQuestion in request.ProductWithQuestionRqs)
                {
                    foreach (var gameBoardQuestionId in productWithQuestion.QuestionIds)
                    {
                        gameBoardQuestionIds.Add(gameBoardQuestionId);
                    }
                }

                List<GameBoardsQuestion> gameBoardQuestions = new List<GameBoardsQuestion>();
                foreach (var questionId in gameBoardQuestionIds)
                {
                    gameBoardQuestions.Add(new GameBoardsQuestion { GameBoardId = request.Id, QuestionId = questionId });
                }

                if (gameBoardQuestions.Count > 0)
                {
                    db.GameBoardsQuestions.AddRange(gameBoardQuestions);
                }

                db.SaveChanges();

                List<ProductsForBoard> targetProductsForBoard = (from p in db.ProductsForBoards where p.GameBoardId == request.Id select p).ToList();
                if (targetProductsForBoard.Count > 0)
                {
                    db.ProductsForBoards.RemoveRange(targetProductsForBoard);
                }

                List<ProductsForBoard> productsForBoards = new List<ProductsForBoard>();
                foreach (var productWithQuestion in request.ProductWithQuestionRqs)
                {
                    productsForBoards.Add(new ProductsForBoard() { GameBoardId = request.Id, ProductId = productWithQuestion.ProductId, NumOfRepeating = productWithQuestion.NumberOfRepeating });
                }

                if (productsForBoards.Count > 0)
                {
                    db.ProductsForBoards.AddRange(productsForBoards);
                }

                db.SaveChanges();
            });

            //Установка вопроса
            app.MapPost("/game/chooseQuestion", (eco_questContext db, GameChooseQuestionRequest request) =>
            {
                Console.WriteLine("==========/game/chooseQuestion==========");

                Session session = (from s in db.Sessions select s).ToArray().First();
                session.IdCurrentQuestion = request.QuestionId;
                db.SaveChanges();
            });

            //Получение вопроса и ответа
            app.MapGet("/game/getAnswer", (eco_questContext db) =>
            {
                Console.WriteLine("==========/game/getAnswer==========");

                long? idCurrentQuestion = (from s in db.Sessions select s.IdCurrentQuestion).ToArray().First();
                Question? question = (from q in db.Questions where q.Id == idCurrentQuestion select q).ToArray().FirstOrDefault();

                return Results.Json(new { Answer = question.Answer, Question = question.Text });
            });

            //Создание нового продукта (вместе с вопросами)
            app.MapPost("/product/create", (eco_questContext db, Product request) =>
            {
                Console.WriteLine("==========/product/create==========");
                
                db.Products.Add(request);
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
            app.MapPost("/product/update", (eco_questContext db, Product request) =>
            {
                Console.WriteLine("==========/product/update==========");
                
                List<Product> allProducts = db.Products.ToList();

                var targetProductSearch = (from p in allProducts where p.Id == request.Id select p).ToList();

                if (targetProductSearch.Count == 0)
                {
                    db.Products.Add(request);
                }
                else if (targetProductSearch.Count == 1)
                {
                    Product targetProduct = targetProductSearch[0];
                    targetProduct.Colour = request.Colour;
                    targetProduct.Name = request.Name;

                    foreach (var newQuestion in request.Questions)
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

        public static string Encrypt(string unencryptedString)
        {
            MD5 hash = MD5.Create();
            byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(unencryptedString));
            StringBuilder encryptedBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                encryptedBuilder.Append(bytes[i].ToString("x2"));
            }
            return encryptedBuilder.ToString();
        }
    }

    public class AdminLeadersWaitingResponse
    {
        public long UserId { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Patronymic { get; set; }
        public string? Login { get; set; }
    }

    public class AuthenticationOptions
    {
        public const string ISSUER = "Backend";
        public const string AUDIENCE = "Frontend";
        private const string KEY = "MySuperSecretKey";

        public static SymmetricSecurityKey GetSymmetricSecurityKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }

    public class AuthLoginMasterRequest
    {
        public string? Login { get; set; }
        public string? Password { get; set; }
    }

    public class AuthLoginPlayerRequest
    {
        public long? GameId { get; set; }
        public string? Login { get; set; }
    }

    public class BoardCreateRequest
    {
        public BoardCreateRequest()
        {
            ProductWithQuestionRqs = new HashSet<ProductWithQuestionRqsResponse>();
        }

        public long Id { get; set; }
        public string? Name { get; set; }
        public virtual ICollection<ProductWithQuestionRqsResponse> ProductWithQuestionRqs { get; set; }
        public int? NumFields { get; set; }
    }

    public class ProductWithQuestionRqsResponse
    {
        public ProductWithQuestionRqsResponse()
        {
            QuestionIds = new HashSet<long>();
        }

        public long ProductId { get; set; }
        public int? NumberOfRepeating { get; set; }
        public virtual ICollection<long> QuestionIds { get; set; }
    }

    public class BoardGetResponse
    {
        public BoardGetResponse()
        {
            Products = new HashSet<ProductResponse>();
        }

        public long Id { get; set; }
        public string? Name { get; set; }
        public virtual ICollection<ProductResponse> Products { get; set; }
        public int? NumFields { get; set; }
    }

    public class ProductResponse
    {
        public ProductResponse()
        {
            Questions = new HashSet<Question>();
        }

        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Colour { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public int? NumOfRepeating { get; set; }
    }

    public class GameChooseQuestionRequest
    {
        public long? QuestionId { get; set; }
        public string? QuestionType { get; set; }
        public string? State { get; set; }
    }
}