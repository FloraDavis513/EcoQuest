using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EcoQuest
{
    public class ApplicationService
    {
        public IResult AuthenticationLoginMaster(eco_questContext db, LoginMasterDTO request)
        {
            Console.WriteLine("==========/authentication/login/master==========");

            User? user = (from u in db.Users where u.Login == request.Login select u).FirstOrDefault();

            if (user == null)
            {
                return Results.NotFound("Пользователя с таким логином не существует");
            }
            if (user.Password != PasswordHasher.Encrypt(request.Password))
            {
                return Results.Unauthorized();
            }

            List<Claim> claims = new List<Claim>() { new Claim(ClaimTypes.Name, user.Login), new Claim(ClaimTypes.Role, $"{user.Role + user.Status}") };
            JwtSecurityToken JWT = new JwtSecurityToken(issuer: AuthenticationOptions.ISSUER, audience: AuthenticationOptions.AUDIENCE, claims: claims, expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                signingCredentials: new SigningCredentials(AuthenticationOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            string encodedJWT = new JwtSecurityTokenHandler().WriteToken(JWT);

            return Results.Json(new { Login = user.Login, AuthorizationToken = encodedJWT });
        }
        public IResult AuthenticationLoginPlayer(eco_questContext db, LoginPlayerDTO request)
        {
            Console.WriteLine("==========/authentication/login/player==========");
            /*
            Game? game = (from g in db.Games where g.GameId == request.GameId select g).FirstOrDefault();

            if (game == null)
            {
                return Results.NotFound("Запраиваемая игра не существует");
            }
            */
            List<Claim> claims = new List<Claim>() { new Claim(ClaimTypes.Name, request.Login), new Claim(ClaimTypes.Role, "player") };
            JwtSecurityToken JWT = new JwtSecurityToken(issuer: AuthenticationOptions.ISSUER, audience: AuthenticationOptions.AUDIENCE, claims: claims, expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                signingCredentials: new SigningCredentials(AuthenticationOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            string encodedJWT = new JwtSecurityTokenHandler().WriteToken(JWT);

            return Results.Json(new { GameId = request.GameId, Login = request.Login, AuthorizationToken = encodedJWT });
        }

        public IResult BoardCreate(eco_questContext db, GameBoardDTO<ProductFoBoardRequestDTO> request)
        {
            Console.WriteLine("==========/board/create==========");

            GameBoard newGameBoard = new GameBoard() { Name = request.Name, NumFields = request.NumFields };

            db.GameBoards.Add(newGameBoard);

            db.SaveChanges();

            GameBoard? addedGameBoard = (from gb in db.GameBoards where gb.Name == newGameBoard.Name select gb).FirstOrDefault();

            if (addedGameBoard == null)
            {
                return Results.StatusCode(500);
            }

            foreach (var product in request.Products)
            {
                foreach (var questionId in product.QuestionIds)
                {
                    db.GameBoardsQuestions.Add(new GameBoardsQuestion() { GameBoardId = addedGameBoard.Id, QuestionId = questionId });
                }
            }

            foreach (var product in request.Products)
            {
                db.ProductsForBoards.Add(new ProductsForBoard() { GameBoardId = addedGameBoard.Id, ProductId = product.ProductId, NumOfRepeating = product.NumberOfRepeating });
            }

            db.SaveChanges();

            return Results.Ok();
        }
        public IResult BoardDeleteId(eco_questContext db, long id)
        {
            Console.WriteLine("==========/board/delete/{id:long}==========");

            List<GameBoard> targetGameBoards = (from gb in db.GameBoards where gb.Id == id select gb).ToList();
            List<GameBoardsQuestion> targetGameBoardsQuestions = (from q in db.GameBoardsQuestions where q.GameBoardId == id select q).ToList();
            List<ProductsForBoard> targetProductsForBoards = (from p in db.ProductsForBoards where p.GameBoardId == id select p).ToList();

            db.GameBoards.RemoveRange(targetGameBoards);
            db.GameBoardsQuestions.RemoveRange(targetGameBoardsQuestions);
            db.ProductsForBoards.RemoveRange(targetProductsForBoards);

            db.SaveChanges();

            return Results.Ok();
        }
        public IResult BoardGetId(eco_questContext db, long id)
        {
            Console.WriteLine("==========/board/get/{id:long}==========");

            GameBoard? targetGameBoard = (from gb in db.GameBoards where gb.Id == id select gb).FirstOrDefault();

            if (targetGameBoard == null)
            {
                return Results.NotFound("Запрашиваемый шаблон не найден");
            }

            GameBoardDTO<ProductForBoardResponseDTO> response = new GameBoardDTO<ProductForBoardResponseDTO>()
            {
                Id = targetGameBoard.Id,
                Name = targetGameBoard.Name,
                NumFields = targetGameBoard.NumFields
            };

            List<ProductsForBoard> targetProductsForBoards = (from p in db.ProductsForBoards where p.GameBoardId == id select p).ToList();

            foreach (var product in targetProductsForBoards)
            {
                Product? targetProduct = (from p in db.Products where p.Id == product.ProductId select p).FirstOrDefault();

                if (targetProduct != null)
                {
                    List<long> targetGameBoardsQuestionsIds = (from q in db.GameBoardsQuestions where q.GameBoardId == id select q.QuestionId).ToList();
                    List<Question> targetProductQuestoins = (from q in db.Questions where q.ProductId == product.ProductId select q).ToList();
                    List<Question> targetQuestions = (from q in targetProductQuestoins where targetGameBoardsQuestionsIds.Contains(q.Id) select q).ToList();

                    ProductForBoardResponseDTO productForBoardResponse = new ProductForBoardResponseDTO()
                    {
                        Id = product.ProductId,
                        Name = targetProduct.Name,
                        Colour = targetProduct.Colour,
                        NumOfRepeating = product.NumOfRepeating
                    };

                    foreach (var question in targetQuestions)
                    {
                        productForBoardResponse.Questions.Add(question);
                    }

                    response.Products.Add(productForBoardResponse);
                }
            }

            return Results.Json(response);
        }
        public IResult BoardGetAll(eco_questContext db)
        {
            Console.WriteLine("==========/board/getAll==========");

            List<GameBoard> allGameBoards = db.GameBoards.ToList();

            return Results.Json(allGameBoards);
        }
        public IResult BoardUpdate(eco_questContext db, GameBoardDTO<ProductFoBoardRequestDTO> request)
        {
            Console.WriteLine("==========/board/update==========");

            GameBoard? targetGameBoard = (from gb in db.GameBoards where gb.Id == request.Id select gb).FirstOrDefault();

            if (targetGameBoard == null)
            {
                BoardDeleteId(db, request.Id);
                return BoardCreate(db, request);
            }

            List<GameBoardsQuestion> targetGameBoardsQuestions = (from q in db.GameBoardsQuestions where q.GameBoardId == request.Id select q).ToList();
            List<ProductsForBoard> targetProductsForBoards = (from p in db.ProductsForBoards where p.GameBoardId == request.Id select p).ToList();

            db.GameBoardsQuestions.RemoveRange(targetGameBoardsQuestions);
            db.ProductsForBoards.RemoveRange(targetProductsForBoards);

            db.SaveChanges();

            targetGameBoard.Name = request.Name;
            targetGameBoard.NumFields = request.NumFields;

            List<GameBoardsQuestion> gameBoardsQuestions = new List<GameBoardsQuestion>();
            List<ProductsForBoard> productsForBoards = new List<ProductsForBoard>();

            foreach (var product in request.Products)
            {
                foreach (var questionId in product.QuestionIds)
                {
                    gameBoardsQuestions.Add(new GameBoardsQuestion { GameBoardId = request.Id, QuestionId = questionId });
                }
            }
            foreach (var product in request.Products)
            {
                productsForBoards.Add(new ProductsForBoard() { GameBoardId = request.Id, ProductId = product.ProductId, NumOfRepeating = product.NumberOfRepeating });
            }

            db.GameBoardsQuestions.AddRange(gameBoardsQuestions);
            db.ProductsForBoards.AddRange(productsForBoards);

            db.SaveChanges();

            return Results.Ok();
        }

        public IResult GameGetAnswer(eco_questContext db)
        {
            Console.WriteLine("==========/game/getAnswer==========");

            Session? session = (from s in db.Sessions select s).FirstOrDefault();

            if (session == null)
            {
                return Results.NotFound("Запрашиваемая сессия не найдена");
            }

            Question? question = (from q in db.Questions where q.Id == session.IdCurrentQuestion select q).FirstOrDefault();

            if (question == null)
            {
                return Results.NotFound("Запрашиваемый вопрос не найден");
            }

            return Results.Json(new { Answer = question.Answer, Question = question.Text });
        }
        public IResult GameSetQuestion(eco_questContext db, SetQuestionDTO request)
        {
            Console.WriteLine("==========/game/setQuestion==========");

            Session? session = (from s in db.Sessions select s).FirstOrDefault();

            if (session == null)
            {
                return Results.NotFound("Запрашиваемая сессия не найдена");
            }

            session.IdCurrentQuestion = request.QuestionId;

            db.SaveChanges();

            return Results.Ok();
        }

        public IResult ProductCreate(eco_questContext db, Product request)
        {
            Console.WriteLine("==========/product/create==========");

            db.Products.Add(request);

            db.SaveChanges();

            return Results.Ok();
        }
        public IResult ProductDeleteId(eco_questContext db, long id)
        {
            Console.WriteLine("==========/product/delete/{id:long}==========");

            List<Product> targetProducts = (from p in db.Products where p.Id == id select p).ToList();
            List<Question> targetQuestions = (from q in db.Questions where q.ProductId == id select q).ToList();
            List<ProductsForBoard> targetProductsForBoards = (from p in db.ProductsForBoards where p.ProductId == id select p).ToList();

            List<long> targetQuestionIds = (from q in targetQuestions select q.Id).ToList();
            List<GameBoardsQuestion> targetGameBoardsQuestions = (from q in db.GameBoardsQuestions where targetQuestionIds.Contains(q.QuestionId) select q).ToList();

            db.Products.RemoveRange(targetProducts);
            db.Questions.RemoveRange(targetQuestions);
            db.ProductsForBoards.RemoveRange(targetProductsForBoards);
            db.GameBoardsQuestions.RemoveRange(targetGameBoardsQuestions);

            db.SaveChanges();

            return Results.Ok();
        }
        public IResult ProductGetAll(eco_questContext db)
        {
            Console.WriteLine("==========/product/getAll==========");

            List<Product> allProducts = db.Products.ToList();
            List<Question> allQuestions = db.Questions.ToList();

            foreach (var product in allProducts)
            {
                List<Question> targetQuestions = (from q in allQuestions where q.ProductId == product.Id select q).ToList();

                foreach (var targetQuestion in targetQuestions)
                {
                    product.Questions.Add(targetQuestion);
                }
            }

            return Results.Json(allProducts);
        }
        public IResult ProductGetAllRound(eco_questContext db, int round)
        {
            Console.WriteLine("==========/product/getAll/{round:int}==========");

            List<Product> targetProducts = (from p in db.Products where p.Round == round select p).ToList();

            List<Question> allQuestions = db.Questions.ToList();

            foreach (var targetProduct in targetProducts)
            {
                List<Question> targetQuestions = (from q in allQuestions where q.ProductId == targetProduct.Id select q).ToList();

                foreach (var targetQuestion in targetQuestions)
                {
                    targetProduct.Questions.Add(targetQuestion);
                }
            }

            return Results.Json(targetProducts);
        }
        public IResult ProductUpdate(eco_questContext db, Product request)
        {
            Console.WriteLine("==========/product/update==========");

            Product? targetProduct = (from p in db.Products where p.Id == request.Id select p).FirstOrDefault();

            if (targetProduct == null)
            {
                ProductDeleteId(db, request.Id);
                return ProductCreate(db, request);
            }

            targetProduct.Colour = request.Colour;
            targetProduct.Name = request.Name;

            foreach (var updatedQuestion in request.Questions)
            {
                Question? targetQuestion = (from q in targetProduct.Questions where q.Id == updatedQuestion.Id select q).FirstOrDefault();

                if (targetQuestion == null)
                {
                    targetProduct.Questions.Add(updatedQuestion);
                }
                else
                {
                    targetQuestion.Answer = updatedQuestion.Answer;
                    targetQuestion.Type = updatedQuestion.Type;
                    targetQuestion.ShortText = updatedQuestion.ShortText;
                    targetQuestion.Text = updatedQuestion.Text;
                }
            }

            db.SaveChanges();

            return Results.Ok();
        }

        public IResult QuestionDeleteId(eco_questContext db, long id)
        {
            Console.WriteLine("==========/question/delete/{id:long}==========");

            List<Question> targetQuestions = (from q in db.Questions where q.Id == id select q).ToList();
            List<GameBoardsQuestion> targetGameBoardsQuestions = (from q in db.GameBoardsQuestions where q.QuestionId == id select q).ToList();

            db.Questions.RemoveRange(targetQuestions);
            db.GameBoardsQuestions.RemoveRange(targetGameBoardsQuestions);

            db.SaveChanges();

            return Results.Ok();
        }

        public IResult UserCreate(eco_questContext db, User request)
        {
            Console.WriteLine("==========/user/create==========");

            List<User> loginExists = (from u in db.Users where u.Login == request.Login select u).ToList();

            if (loginExists.Count > 0)
            {
                return Results.BadRequest("Логин уже существует");
            }

            User newUser = new User()
            {
                LastName = request.LastName,
                FirstName = request.FirstName,
                Patronymic = request.Patronymic,
                Login = request.Login,
                Password = PasswordHasher.Encrypt(request.Password),
                Role = "master",
                Status = "inactive"
            };

            db.Users.Add(newUser);

            db.SaveChanges();

            return Results.Ok();
        }
        public IResult UserDeleteId(eco_questContext db, long id)
        {
            Console.WriteLine("==========/user/delete/{id:long}==========");

            List<User> targetUsers = (from u in db.Users where u.UserId == id select u).ToList();

            db.Users.RemoveRange(targetUsers);
            db.SaveChanges();

            return Results.Ok();
        }
        public IResult UserGetActiveMasters(eco_questContext db)
        {
            Console.WriteLine("==========/user/getActiveMasters==========");

            return Results.Json((from u in db.Users where u.Role == "master" && u.Status == "active" select u).ToList());
        }
        public IResult UserGetInactiveMasters(eco_questContext db)
        {
            Console.WriteLine("==========/user/getInactiveMasters==========");

            return Results.Json((from u in db.Users where u.Role == "master" && u.Status == "inactive" select u).ToList());
        }
        public IResult UserToActiveMasterId(eco_questContext db, long id)
        {
            Console.WriteLine("==========/user/toActiveMaster/{id:long}==========");

            List<User> targetUsers = (from u in db.Users where u.UserId == id select u).ToList();

            if (targetUsers.Count == 0)
            {
                return Results.NotFound("Запрашиваемый пользователь не найден");
            }

            foreach (var targetUser in targetUsers)
            {
                targetUser.Status = "active";
            }

            db.SaveChanges();

            return Results.Ok();
        }
    }
}