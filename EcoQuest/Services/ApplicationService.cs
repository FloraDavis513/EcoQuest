using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Numerics;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using static EcoQuest.StatisticsResultsDTO;

namespace EcoQuest
{
    public class ApplicationService
    {
        public ApplicationService(WebApplication app)
        {
            _app = app;
        }

        private readonly WebApplication _app;

        public IResult AuthenticationLoginMaster(eco_questContext db, LoginMasterDTO request)
        {
            Console.WriteLine("==========/authentication/login/master==========");

            (bool, IResult) validResult = RequestValidator.ValidateLoginMasterDTO(db, request);

            if (!validResult.Item1)
                return validResult.Item2;

            User? targetUser = (from u in db.Users
                                where u.Login == request.Login
                                select u).FirstOrDefault();

            if (targetUser == null)
                return Results.NotFound("Пользователь с запрашиваемым логином не найден");
            if (targetUser.Password != PasswordHasher.Encrypt(request.Password))
                return Results.Unauthorized();

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, targetUser.Login),
                new Claim(ClaimTypes.Role, $"{targetUser.Role + targetUser.Status}")
            };

            JwtSecurityToken JWT = new JwtSecurityToken(
                issuer: AuthenticationOptions.ISSUER,
                audience: AuthenticationOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromDays(30)),
                signingCredentials: new SigningCredentials(AuthenticationOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            string encodedJWT = new JwtSecurityTokenHandler().WriteToken(JWT);

            return Results.Json(new
            {
                UserId = targetUser.UserId,
                Login = targetUser.Login,
                Role = targetUser.Role,
                Status = targetUser.Status,
                Name = targetUser.LastName + " " + targetUser.FirstName + " " + targetUser.Patronymic,
                AuthorizationToken = encodedJWT
            });
        }
        public IResult AuthenticationLoginPlayer(eco_questContext db, LoginPlayerDTO request)
        {
            Console.WriteLine("==========/authentication/login/player==========");

            DeleteExpiredGames(db);

            (bool, IResult) validResult = RequestValidator.ValidateLoginPlayerDTO(db, request);

            if (!validResult.Item1)
                return validResult.Item2;

            Game? targetGame = (from g in db.Games
                                where g.GameId == request.GameId
                                select g).FirstOrDefault();

            if (targetGame == null)
                return Results.NotFound("Запрашиваемая игра не найдена");

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, request.Login),
                new Claim(ClaimTypes.Role, "player")
            };

            JwtSecurityToken JWT = new JwtSecurityToken(
                issuer: AuthenticationOptions.ISSUER,
                audience: AuthenticationOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromDays(30)),
                signingCredentials: new SigningCredentials(AuthenticationOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            string encodedJWT = new JwtSecurityTokenHandler().WriteToken(JWT);

            return Results.Json(new
            {
                GameId = request.GameId,
                Login = request.Login,
                Role = "player",
                AuthorizationToken = encodedJWT
            });
        }

        public IResult GameCreate(eco_questContext db, Game request)
        {
            Console.WriteLine("==========/game/create==========");

            DeleteExpiredGames(db);

            (bool, IResult) validResult = RequestValidator.ValidateGameModel(db, request);

            if (!validResult.Item1)
                return validResult.Item2;

            List<long> allGameIds = (from g in db.Games
                                     select g.GameId).ToList();

            long gameId = 0;

            for (int id = 1; id <= 99999; id++)
            {
                if (!allGameIds.Contains(id))
                {
                    gameId = id;
                    break;
                }
            }

            if (gameId == 0)
                return Results.BadRequest("Пул игр переполнен");

            Game newGame = new Game()
            {
                GameId = gameId,
                UserId = request.UserId,
                Name = request.Name,
                Message = request.Message,
                Date = request.Date,
                State = request.State,
                CurrentQuestionId = request.CurrentQuestionId,
            };

            db.Games.Add(newGame);

            db.SaveChanges();

            return Results.Ok();
        }
        public IResult GameDeleteId(eco_questContext db, long id)
        {
            Console.WriteLine("==========/game/delete/{id:long}==========");

            DeleteExpiredGames(db);

            Game? targetGame = (from g in db.Games
                                where g.GameId == id
                                select g).FirstOrDefault();

            if (targetGame != null)
                db.Games.Remove(targetGame);

            db.SaveChanges();

            return Results.Ok();
        }
        public IResult GameGetId(eco_questContext db, long id)
        {
            Console.WriteLine("==========/game/get/{id:long}==========");

            DeleteExpiredGames(db);

            Game? targetGame = (from g in db.Games
                                where g.GameId == id
                                select g).FirstOrDefault();

            if (targetGame == null)
                return Results.NotFound("Запрашиваемая игра не найдена");

            Question? targetQuestion = (from q in db.Questions
                                        where q.QuestionId == targetGame.CurrentQuestionId
                                        select q).FirstOrDefault();

            return Results.Json(new
            {
                GameId = targetGame.GameId,
                UserId = targetGame.UserId,
                Name = targetGame.Name,
                Message = targetGame.Message,
                Date = targetGame.Date,
                State = targetGame.State,
                CurrentQuestionId = targetGame.CurrentQuestionId,
                CurrentQuestionAnswer = targetQuestion == null ? null : targetQuestion.Answers
            });
        }
        public IResult GameGetAll(eco_questContext db)
        {
            Console.WriteLine("==========/game/get/all==========");

            DeleteExpiredGames(db);

            List<Game> allGames = db.Games.ToList();

            foreach (var game in allGames)
            {
                game.CurrentQuestionId = null;
            }

            return Results.Json(allGames.OrderBy(x => x.GameId));
        }
        public IResult GameGetAllId(eco_questContext db, long id)
        {
            Console.WriteLine("==========/game/get/all/{id:long}==========");

            DeleteExpiredGames(db);

            User? targetUser = (from u in db.Users.Include(x => x.Games)
                                where u.UserId == id
                                select u).FirstOrDefault();

            if (targetUser == null)
                return Results.NotFound("Запрашиваемый пользователь не найден");
            if (!(targetUser.Role == "master" && targetUser.Status == "active"))
                return Results.BadRequest("Запрашиваемый пользователь не является активным ведущим");

            foreach (var game in targetUser.Games)
            {
                game.CurrentQuestionId = null;
                game.User = null;
            }

            return Results.Json(targetUser.Games.OrderBy(x => x.GameId));
        }
        public IResult GameStatePlayersCreateId(eco_questContext db, PlayerDTO request, long id)
        {
            Console.WriteLine("==========/game/state/players/create/{id:long}==========");

            DeleteExpiredGames(db);

            Game? targetGame = (from g in db.Games
                                where g.GameId == id
                                select g).FirstOrDefault();

            if (targetGame == null)
                return Results.NotFound("Запрашиваемая игра не найдена");
            if (targetGame.State == null)
                return Results.BadRequest("Поле State имеет значение null");

            JsonNode? stateNode = JsonNode.Parse(targetGame.State);

            if (stateNode != null)
            {
                JsonNode? playersNode = stateNode["Players"];

                if (playersNode != null)
                {
                    JsonArray allPlayers = playersNode.AsArray();

                    List<long> allPlayerIds = (from p in allPlayers
                                               select (long)p["PlayerId"]!).ToList();

                    long newPlayerId = 1;

                    while (allPlayerIds.Contains(newPlayerId))
                        newPlayerId++;

                    PlayerDTO newPlayer = new PlayerDTO()
                    {
                        PlayerId = newPlayerId,
                        Login = request.Login,
                        List = request.List
                    };

                    allPlayers.Add(newPlayer);

                    JsonObject stateObject = stateNode.AsObject();

                    stateObject["Players"] = allPlayers;

                    targetGame.State = stateObject.ToJsonString(new JsonSerializerOptions()
                    {
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                        WriteIndented = true
                    });

                    db.SaveChanges();
                }
                else
                {
                    return Results.BadRequest("Поле Players имеет значение null");
                }
            }
            else
            {
                return Results.BadRequest("Поле State имеет значение null");
            }

            return Results.Ok();
        }
        public IResult GameStatePlayersDeleteGameIdPlayerId(eco_questContext db, long gameId, long playerId)
        {
            Console.WriteLine("==========/game/state/players/delete/{gameId:long}/{playerId:long}==========");

            DeleteExpiredGames(db);

            Game? targetGame = (from g in db.Games
                                where g.GameId == gameId
                                select g).FirstOrDefault();

            if (targetGame == null)
                return Results.NotFound("Запрашиваемая игра не найдена");
            if (targetGame.State == null)
                return Results.BadRequest("Поле State имеет значение null");

            JsonNode? stateNode = JsonNode.Parse(targetGame.State);

            if (stateNode != null)
            {
                JsonNode? playersNode = stateNode["Players"];

                if (playersNode != null)
                {
                    JsonArray allPlayers = playersNode.AsArray();

                    JsonNode? targetPlayerNode = (from p in allPlayers
                                                  where (long)p["PlayerId"]! == playerId
                                                  select p).FirstOrDefault();

                    if (targetPlayerNode != null)
                    {
                        allPlayers.Remove(targetPlayerNode);

                        JsonObject stateObject = stateNode.AsObject();

                        stateObject["Players"] = allPlayers;

                        targetGame.State = stateObject.ToJsonString(new JsonSerializerOptions()
                        {
                            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                            WriteIndented = true
                        });

                        db.SaveChanges();
                    }
                }
                else
                {
                    return Results.BadRequest("Поле Players имеет значение null");
                }
            }
            else
            {
                return Results.BadRequest("Поле State имеет значение null");
            }

            return Results.Ok();
        }
        public IResult GameStatePlayersUpdateId(eco_questContext db, PlayerDTO request, long id)
        {
            Console.WriteLine("==========/game/state/players/update/{id:long}==========");

            DeleteExpiredGames(db);

            Game? targetGame = (from g in db.Games
                                where g.GameId == id
                                select g).FirstOrDefault();

            if (targetGame == null)
                return Results.NotFound("Запрашиваемая игра не найдена");
            if (targetGame.State == null)
                return Results.BadRequest("Поле State имеет значение null");

            JsonNode? stateNode = JsonNode.Parse(targetGame.State);

            if (stateNode != null)
            {
                JsonNode? playersNode = stateNode["Players"];

                if (playersNode != null)
                {
                    JsonArray allPlayers = playersNode.AsArray();

                    JsonNode? targetPlayerNode = (from p in allPlayers
                                                  where (long)p["PlayerId"]! == request.PlayerId
                                                  select p).FirstOrDefault();

                    if (targetPlayerNode == null)
                        return Results.NotFound("Запрашиваемый игрок не найден");

                    JsonObject targetPlayerObject = targetPlayerNode.AsObject();

                    targetPlayerObject["Login"] = request.Login;
                    targetPlayerObject["List"] = request.List;

                    targetGame.State = stateNode.ToJsonString(new JsonSerializerOptions()
                    {
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                        WriteIndented = true
                    });

                    db.SaveChanges();
                }
                else
                {
                    return Results.BadRequest("Поле Players имеет значение null");
                }
            }
            else
            {
                return Results.BadRequest("Поле State имеет значение null");
            }

            return Results.Ok();
        }
        public IResult GameUpdate(eco_questContext db, Game request)
        {
            Console.WriteLine("==========/game/update==========");

            DeleteExpiredGames(db);

            (bool, IResult) validResult = RequestValidator.ValidateGameModel(db, request);

            if (!validResult.Item1)
                return validResult.Item2;

            Game? targetGame = (from g in db.Games
                                where g.GameId == request.GameId
                                select g).FirstOrDefault();

            if (targetGame == null)
                return Results.NotFound("Запрашиваемая игра не найдена");

            targetGame.UserId = request.UserId;
            targetGame.Name = request.Name;
            targetGame.Message = request.Message;
            targetGame.Date = request.Date;
            targetGame.State = request.State;
            targetGame.CurrentQuestionId = request.CurrentQuestionId;

            db.SaveChanges();

            return Results.Ok();
        }
        public IResult GameUpdateStateAndQuestion(eco_questContext db, Game request)
        {
            Console.WriteLine("==========/game/update/stateAndQuestion==========");

            DeleteExpiredGames(db);

            Game? targetGame = (from g in db.Games
                                where g.GameId == request.GameId
                                select g).FirstOrDefault();

            if (targetGame == null)
                return Results.NotFound("Запрашиваемая игра не найдена");

            targetGame.State = request.State;
            targetGame.CurrentQuestionId = request.CurrentQuestionId;

            db.SaveChanges();

            return Results.Ok();
        }

        public IResult GameBoardCreate(eco_questContext db, GameBoardDTO request)
        {
            Console.WriteLine("==========/gameBoard/create==========");

            GameBoard convertedRequest = ModelConverter.ToGameBoard(db, request);

            (bool, IResult) validResult = RequestValidator.ValidateGameBoardModel(db, convertedRequest);

            if (!validResult.Item1)
                return validResult.Item2;

            GameBoard newGameBoard = new GameBoard()
            {
                Name = convertedRequest.Name,
                NumFields = convertedRequest.NumFields,
                UserId = convertedRequest.UserId,
            };

            foreach (var gameBoardsProduct in convertedRequest.GameBoardsProducts)
            {
                GameBoardsProduct newGameBoardsProduct = new GameBoardsProduct()
                {
                    ProductId = gameBoardsProduct.ProductId,
                    NumOfRepeating = gameBoardsProduct.NumOfRepeating
                };

                newGameBoard.GameBoardsProducts.Add(newGameBoardsProduct);
            }

            List<Question> allQuestions = db.Questions.ToList();

            foreach (var gameBoardsQuestion in convertedRequest.Questions)
            {
                Question? targetQuestion = (from q in allQuestions
                                            where q.QuestionId == gameBoardsQuestion.QuestionId
                                            select q).FirstOrDefault();

                if (targetQuestion != null)
                    newGameBoard.Questions.Add(targetQuestion);
            }

            db.GameBoards.Add(newGameBoard);

            db.SaveChanges();

            return Results.Ok();
        }
        public IResult GameBoardDeleteId(eco_questContext db, long id)
        {
            Console.WriteLine("==========/gameBoard/delete/{id:long}==========");

            GameBoard? targetGameBoard = (from gb in db.GameBoards
                                          where gb.GameBoardId == id
                                          select gb).FirstOrDefault();

            if (targetGameBoard != null)
                db.GameBoards.Remove(targetGameBoard);

            db.SaveChanges();

            return Results.Ok();
        }
        public IResult GameBoardGetId(eco_questContext db, long id)
        {
            Console.WriteLine("==========/gameBoard/get/{id:long}==========");

            GameBoard? targetGameBoard = (from gb in db.GameBoards.Include(x => x.GameBoardsProducts).ThenInclude(y => y.Product).Include(z => z.Questions)
                                          where gb.GameBoardId == id
                                          select gb).FirstOrDefault();

            if (targetGameBoard == null)
                return Results.NotFound("Запрашиваемый шаблон не найден");

            foreach (var gameBoardsProduct in targetGameBoard.GameBoardsProducts)
            {
                List<Question> targetQuestions = (from q in targetGameBoard.Questions
                                                  where q.ProductId == gameBoardsProduct.ProductId
                                                  select q).ToList();

                foreach (var question in targetQuestions)
                    gameBoardsProduct.Product.Questions.Add(question);
            }

            GameBoardDTO convertedResponse = ModelConverter.ToGameBoardDTO(db, targetGameBoard);

            return Results.Json(convertedResponse, new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            });
        }
        public IResult GameBoardGetAll(eco_questContext db)
        {
            Console.WriteLine("==========/gameBoard/get/all==========");

            List<GameBoard> allGameBoards = db.GameBoards.ToList();

            List<GameBoardDTO> convertedResponse = (from gb in allGameBoards
                                                    select ModelConverter.ToGameBoardDTO(db, gb)).ToList();

            return Results.Json(convertedResponse.OrderBy(x => x.GameBoardId));
        }
        public IResult GameBoardGetAllId(eco_questContext db, long id)
        {
            Console.WriteLine("==========/gameBoard/get/all/{id:long}==========");

            User? targetUser = (from u in db.Users.Include(x => x.GameBoards)
                                where u.UserId == id
                                select u).FirstOrDefault();

            if (targetUser == null)
                return Results.NotFound("Запрашиваемый пользователь не найден");
            if (!(targetUser.Role == "master" && targetUser.Status == "active"))
                return Results.BadRequest("Запрашиваемый пользователь не является активным ведущим");

            foreach (var gameBoard in targetUser.GameBoards)
                gameBoard.User = null;

            List<GameBoardDTO> convertedResponse = (from gb in targetUser.GameBoards
                                                    select ModelConverter.ToGameBoardDTO(db, gb)).ToList();

            return Results.Json(convertedResponse.OrderBy(x => x.GameBoardId));
        }
        public IResult GameBoardShareFromUserIdGameBoardIdToUserId(eco_questContext db, long fromUserId, long gameBoardId, long toUserId)
        {
            Console.WriteLine("==========/gameBoard/share/{fromUserId:long}/{gameBoardId:long}/{toUserId:long}==========");

            User? targetFromUser = (from u in db.Users
                                    where u.UserId == fromUserId
                                    select u).FirstOrDefault();

            if (targetFromUser == null)
                return Results.NotFound("Запрашиваемый пользователь адресант не найден");
            if (!(targetFromUser.Role == "master" && targetFromUser.Status == "active"))
                return Results.BadRequest("Запрашиваемый пользователь адресант не является активным ведущим");

            GameBoard? targetGameBoard = (from gb in db.GameBoards.Include(x => x.GameBoardsProducts).Include(y => y.Questions)
                                          where gb.GameBoardId == gameBoardId
                                          select gb).FirstOrDefault();

            if (targetGameBoard == null)
                return Results.NotFound("Запрашиваемый шаблон не найден");
            if (targetGameBoard.UserId != targetFromUser.UserId)
                return Results.BadRequest("Запрашиваемый шаблон не связан с запрашиваемым пользователем адресантом");

            User? targetToUser = (from u in db.Users
                                  where u.UserId == toUserId
                                  select u).FirstOrDefault();

            if (targetToUser == null)
                return Results.NotFound("Запрашиваемый пользователь адресат не найден");
            if (!(targetToUser.Role == "master" && targetToUser.Status == "active"))
                return Results.BadRequest("Запрашиваемый пользователь адресат не является активным ведущим");
            if (targetFromUser.UserId == targetToUser.UserId)
                return Results.BadRequest("Нельзя поделиться шаблоном с самим собой");

            GameBoard newGameBoard = new GameBoard()
            {
                Name = targetGameBoard.Name,
                NumFields = targetGameBoard.NumFields,
                UserId = targetToUser.UserId,
            };

            foreach (var gameBoardsProduct in targetGameBoard.GameBoardsProducts)
            {
                GameBoardsProduct newGameBoardsProduct = new GameBoardsProduct()
                {
                    ProductId = gameBoardsProduct.ProductId,
                    NumOfRepeating = gameBoardsProduct.NumOfRepeating
                };

                newGameBoard.GameBoardsProducts.Add(newGameBoardsProduct);
            }

            List<Question> allQuestions = db.Questions.ToList();

            foreach (var gameBoardsQuestion in targetGameBoard.Questions)
            {
                Question? targetQuestion = (from q in allQuestions
                                            where q.QuestionId == gameBoardsQuestion.QuestionId
                                            select q).FirstOrDefault();

                if (targetQuestion != null)
                    newGameBoard.Questions.Add(targetQuestion);
            }

            db.GameBoards.Add(newGameBoard);

            db.SaveChanges();

            return Results.Ok();
        }
        public IResult GameBoardUpdate(eco_questContext db, GameBoardDTO request)
        {
            Console.WriteLine("==========/gameBoard/update==========");

            GameBoard convertedRequest = ModelConverter.ToGameBoard(db, request);

            (bool, IResult) validResult = RequestValidator.ValidateGameBoardModel(db, convertedRequest);

            if (!validResult.Item1)
                return validResult.Item2;

            GameBoard? targetGameBoard = (from gb in db.GameBoards.Include(x => x.GameBoardsProducts).Include(y => y.Questions)
                                          where gb.GameBoardId == convertedRequest.GameBoardId
                                          select gb).FirstOrDefault();

            if (targetGameBoard == null)
            {
                GameBoardDeleteId(db, convertedRequest.GameBoardId);
                return GameBoardCreate(db, request);
            }

            targetGameBoard.Name = convertedRequest.Name;
            targetGameBoard.NumFields = convertedRequest.NumFields;
            targetGameBoard.UserId = convertedRequest.UserId;

            targetGameBoard.GameBoardsProducts.Clear();
            targetGameBoard.Questions.Clear();

            db.SaveChanges();

            foreach (var gameBoardsProduct in convertedRequest.GameBoardsProducts)
            {
                GameBoardsProduct newGameBoardsProduct = new GameBoardsProduct()
                {
                    GameBoardId = gameBoardsProduct.GameBoardId,
                    ProductId = gameBoardsProduct.ProductId,
                    NumOfRepeating = gameBoardsProduct.NumOfRepeating
                };

                targetGameBoard.GameBoardsProducts.Add(newGameBoardsProduct);
            }

            List<Question> allQuestions = db.Questions.ToList();

            foreach (var gameBoardsQuestion in convertedRequest.Questions)
            {
                Question? targetQuestion = (from q in allQuestions
                                            where q.QuestionId == gameBoardsQuestion.QuestionId
                                            select q).FirstOrDefault();

                if (targetQuestion != null)
                    targetGameBoard.Questions.Add(targetQuestion);
            }

            db.SaveChanges();

            return Results.Ok();
        }

        public IResult ProductCreate(eco_questContext db, Product request)
        {
            Console.WriteLine("==========/product/create==========");

            (bool, IResult) validResult = RequestValidator.ValidateProductModel(db, request);

            if (!validResult.Item1)
                return validResult.Item2;

            Product newProduct = new Product()
            {
                Colour = request.Colour,
                Name = request.Name,
                Round = request.Round,
                Logo = request.Logo
            };

            foreach (var question in request.Questions)
            {
                Question newQuestion = new Question()
                {
                    Answers = question.Answers,
                    Type = question.Type,
                    ShortText = question.ShortText,
                    Text = question.Text,
                    Media = question.Media,
                    LastEditDate = question.LastEditDate,
                };

                if (newQuestion.Type != "MEDIA")
                    newQuestion.Media = null;
                newQuestion.LastEditDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Ekaterinburg Standard Time")).ToString(new CultureInfo("en-US"));

                newProduct.Questions.Add(newQuestion);
            }

            db.Products.Add(newProduct);

            db.SaveChanges();

            return Results.Ok();
        }
        public IResult ProductExport(eco_questContext db, ProductExportDTO request)
        {
            Console.WriteLine("==========/product/export==========");

            (bool, IResult) validResult = RequestValidator.ValidateProductExportDTO(db, request);

            if (!validResult.Item1)
                return validResult.Item2;

            List<Product> targetProducts;

            if (request.ProductIds.Count == 0)
            {
                targetProducts = db.Products.OrderBy(x => x.ProductId).Include(y => y.Questions.OrderBy(z => z.QuestionId)).ToList();
            }
            else
            {
                targetProducts = (from p in db.Products.OrderBy(x => x.ProductId).Include(y => y.Questions.OrderBy(z => z.QuestionId))
                                  where request.ProductIds.Contains(p.ProductId)
                                  select p).ToList();
            }

            XLWorkbook workbook = new XLWorkbook();

            int row = 1;

            foreach (var product in targetProducts)
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add(product.Name);

                worksheet.Cell("A" + row).Value = "Продукт";

                worksheet.Range($"A{row}:E{row}").Style.Fill.BackgroundColor = XLColor.DarkGreen;

                worksheet.Range($"A{row}:E{row}").Style.Font.FontColor = XLColor.White;

                worksheet.Range($"A{row}:E{row}").Merge();

                row++;

                worksheet.Cell("A" + row).Value = "ID продукта";
                worksheet.Cell("B" + row).Value = "Название";
                worksheet.Cell("C" + row).Value = "Цвет";
                worksheet.Cell("D" + row).Value = "Логотип";
                worksheet.Cell("E" + row).Value = "Раунд";

                worksheet.Range($"A{row}:E{row}").Style.Fill.BackgroundColor = XLColor.Green;

                worksheet.Range($"A{row}:E{row}").Style.Font.FontColor = XLColor.White;

                row++;

                worksheet.Cell("A" + row).Value = product.ProductId;
                worksheet.Cell("B" + row).Value = product.Name;
                worksheet.Cell("C" + row).Value = product.Colour;
                worksheet.Cell("D" + row).Value = product.Logo;
                worksheet.Cell("E" + row).Value = product.Round;

                worksheet.Range($"A{row}:E{row}").Style.Fill.BackgroundColor = XLColor.LightGreen;

                row++;
                row++;

                worksheet.Cell("A" + row).Value = "Вопросы";

                worksheet.Range($"A{row}:H{row}").Style.Fill.BackgroundColor = XLColor.DarkGreen;

                worksheet.Range($"A{row}:H{row}").Style.Font.FontColor = XLColor.White;

                worksheet.Range($"A{row}:H{row}").Merge();

                row++;

                worksheet.Cell("A" + row).Value = "ID продукта";
                worksheet.Cell("B" + row).Value = "ID вопроса";
                worksheet.Cell("C" + row).Value = "Категория";
                worksheet.Cell("D" + row).Value = "Краткое обозначение";
                worksheet.Cell("E" + row).Value = "Формулировка";
                worksheet.Cell("F" + row).Value = "Варианты ответов";
                worksheet.Cell("G" + row).Value = "Медиа";
                worksheet.Cell("H" + row).Value = "Дата последнего редактирования";

                worksheet.Range($"A{row}:H{row}").Style.Fill.BackgroundColor = XLColor.Green;

                worksheet.Range($"A{row}:H{row}").Style.Font.FontColor = XLColor.White;

                row++;

                QuestionAnswersDTO? questionAnswersDTO;

                foreach (var question in product.Questions)
                {
                    string? answers = question.Answers;

                    try
                    {
                        questionAnswersDTO = JsonSerializer.Deserialize<QuestionAnswersDTO>(answers);

                        if (questionAnswersDTO == null)
                            questionAnswersDTO = new QuestionAnswersDTO();

                        List<string> allAnswers = questionAnswersDTO.AllAnswers.ToList();
                        List<string> correctAnswers = questionAnswersDTO.CorrectAnswers.ToList();

                        for (int i = 0; i < allAnswers.Count; i++)
                        {
                            if (correctAnswers.Contains(allAnswers[i]))
                                allAnswers[i] = allAnswers[i].Insert(0, "[(*)");
                            else
                                allAnswers[i] = allAnswers[i].Insert(0, "[");
                            allAnswers[i] = allAnswers[i].Insert(allAnswers[i].Length, "]");
                        }

                        answers = string.Join(';', allAnswers);
                    }
                    catch { }

                    worksheet.Cell("A" + row).Value = question.ProductId;
                    worksheet.Cell("B" + row).Value = question.QuestionId;
                    worksheet.Cell("C" + row).Value = question.Type;
                    worksheet.Cell("D" + row).Value = question.ShortText;
                    worksheet.Cell("E" + row).Value = question.Text;
                    worksheet.Cell("F" + row).Value = answers;
                    worksheet.Cell("G" + row).Value = question.Media;
                    worksheet.Cell("H" + row).Value = question.LastEditDate;

                    worksheet.Range($"A{row}:H{row}").Style.Fill.BackgroundColor = XLColor.LightGreen;

                    row++;
                }

                if (row > 1)
                {
                    worksheet.Range($"A1:H{row - 1}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Range($"A1:H{row - 1}").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    worksheet.Range($"A1:H{row - 1}").Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Range($"A1:H{row - 1}").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }

                row = 1;

                worksheet.Cell("J1").Value = "Справочник категорий вопросов";
                worksheet.Cell("J1").Style.Fill.BackgroundColor = XLColor.DarkGreen;
                worksheet.Cell("J1").Style.Font.FontColor = XLColor.White;

                worksheet.Cell("J2").Style.Fill.BackgroundColor = XLColor.Green;
                worksheet.Cell("J2").Style.Font.FontColor = XLColor.White;

                worksheet.Cell("J3").Value = "TEXT - Без выбора ответа";
                worksheet.Cell("J4").Value = "TEXT_WITH_ANSWERS - С выбором ответа";
                worksheet.Cell("J5").Value = "AUCTION - Вопрос-аукцион";
                worksheet.Cell("J6").Value = "MEDIA - Вопрос с медиа фрагментом";
                worksheet.Range($"J3:J6").Style.Fill.BackgroundColor = XLColor.LightGreen;

                worksheet.Range($"J1:J6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range($"J1:J6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                worksheet.Range($"J1:J6").Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range($"J1:J6").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;



                worksheet.Cell("J8").Value = "Шаблон вариантов ответа";
                worksheet.Cell("J8").Style.Fill.BackgroundColor = XLColor.DarkGreen;
                worksheet.Cell("J8").Style.Font.FontColor = XLColor.White;

                worksheet.Cell("J9").Style.Fill.BackgroundColor = XLColor.Green;
                worksheet.Cell("J9").Style.Font.FontColor = XLColor.White;

                worksheet.Cell("J10").Value = "[Неверный ответ];[(*)Верный ответ];[Неверный ответ]";
                worksheet.Cell("J10").Style.Fill.BackgroundColor = XLColor.LightGreen;

                worksheet.Range($"J8:J10").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range($"J8:J10").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                worksheet.Range($"J8:J10").Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range($"J8:J10").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;



                worksheet.Columns().AdjustToContents();
            }

            List<string> oldFiles = (from file in Directory.GetFiles(_app.Configuration["SourcePath"])
                                     where Regex.IsMatch(Path.GetFileName(file), @"^product.*\.xlsx$")
                                     select file).ToList();

            foreach (var oldFile in oldFiles)
            {
                File.Delete(oldFile);
            }

            string filePath = $"{_app.Configuration["SourcePath"]}{request.FileName}.xlsx";

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                workbook.SaveAs(fileStream);
            }

            return Results.Ok();
        }
        public IResult ProductDeleteId(eco_questContext db, long id)
        {
            Console.WriteLine("==========/product/delete/{id:long}==========");

            Product? targetProduct = (from p in db.Products
                                      where p.ProductId == id
                                      select p).FirstOrDefault();

            if (targetProduct != null)
                db.Products.Remove(targetProduct);

            db.SaveChanges();

            return Results.Ok();
        }
        public IResult ProductGetAll(eco_questContext db)
        {
            Console.WriteLine("==========/product/get/all==========");

            List<Product> allProducts = db.Products.Include(x => x.Questions).ToList();

            foreach (var product in allProducts)
            {
                foreach (var question in product.Questions)
                    question.Product = null;
            }

            return Results.Json(allProducts.OrderBy(x => x.ProductId));
        }
        public IResult ProductGetAllRound(eco_questContext db, int round)
        {
            Console.WriteLine("==========/product/get/all/{round:int}==========");

            List<Product> targetProducts = db.Products.Where(x => x.Round == round).Include(y => y.Questions).ToList();

            foreach (var product in targetProducts)
            {
                foreach (var question in product.Questions)
                    question.Product = null;
            }

            return Results.Json(targetProducts.OrderBy(x => x.ProductId));
        }
        public IResult ProductImport(eco_questContext db, HttpRequest request)
        {
            Console.WriteLine("==========/product/import==========");

            IFormFile? file = request.Form.Files.FirstOrDefault();

            if (file == null)
                return Results.BadRequest("Файл не загружен");

            using (Stream stream = file.OpenReadStream())
            {
                XLWorkbook workbook = new XLWorkbook(stream);

                QuestionAnswersDTO questionAnswersDTO;

                foreach (var worksheet in workbook.Worksheets)
                {
                    long productId1;
                    bool productId1ParsingResult = long.TryParse(worksheet.Cell("A3").Value.ToString(), out productId1);
                    if (!productId1ParsingResult)
                        productId1 = 0;

                    string? name = worksheet.Cell("B3").Value.ToString();
                    string? colour = worksheet.Cell("C3").Value.ToString();

                    int round;
                    bool roundParsingResult = int.TryParse(worksheet.Cell("E3").Value.ToString(), out round);
                    if (!roundParsingResult)
                        round = 0;

                    string? logo = worksheet.Cell("D3").Value.ToString();
                    if (!(Path.GetFileNameWithoutExtension(logo) == $"logo{productId1}" && File.Exists(_app.Configuration["SourcePath"] + logo)))
                        logo = null;

                    Product newProduct = new Product()
                    {
                        ProductId = productId1,
                        Colour = colour,
                        Name = name,
                        Round = round,
                        Logo = logo
                    };

                    int row = 7;

                    bool IsRowEmpty(IXLWorksheet worksheet, int row)
                    {
                        bool isRowEmpty = true;

                        char[] columns = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };

                        foreach (var column in columns)
                        {
                            if (!String.IsNullOrEmpty(worksheet.Cell($"{column}{row}").Value.ToString()))
                            {
                                isRowEmpty = false;
                                break;
                            }
                        }

                        return isRowEmpty;
                    }

                    while (!IsRowEmpty(worksheet, row))
                    {
                        long productId2;
                        bool productId2ParsingResult = long.TryParse(worksheet.Cell("A" + row).Value.ToString(), out productId2);
                        if (!productId2ParsingResult)
                            productId2 = 0;

                        long questionId;
                        bool questionIdParsingResult = long.TryParse(worksheet.Cell("B" + row).Value.ToString(), out questionId);
                        if (!questionIdParsingResult)
                            questionId = 0;

                        string? type = worksheet.Cell("C" + row).Value.ToString();
                        string? shortText = worksheet.Cell("D" + row).Value.ToString();
                        string? text = worksheet.Cell("E" + row).Value.ToString();
                        string? answers = worksheet.Cell("F" + row).Value.ToString();
                        string? lastEditDate = worksheet.Cell("H" + row).Value.ToString();

                        string? media = worksheet.Cell("G" + row).Value.ToString();
                        if (!(Path.GetFileNameWithoutExtension(media) == $"media{questionId}" && File.Exists(_app.Configuration["SourcePath"] + media)))
                            media = null;

                        if (answers != null)
                        {
                            questionAnswersDTO = new QuestionAnswersDTO();

                            Regex allAnswersRegex = new Regex(@"\[[^]]+]");
                            Regex correctAnswersRegex = new Regex(@"\[\(\*\)[^]]+]");

                            MatchCollection allAnswersMatches = allAnswersRegex.Matches(answers);
                            MatchCollection correctAnswersMatches = correctAnswersRegex.Matches(answers);

                            questionAnswersDTO.AllAnswers = (from match in allAnswersMatches select match.Value.TrimStart('[').TrimEnd(']').Replace("(*)", "")).ToList();
                            questionAnswersDTO.CorrectAnswers = (from match in correctAnswersMatches select match.Value.TrimStart('[').TrimEnd(']').Replace("(*)", "")).ToList();

                            answers = JsonSerializer.Serialize(questionAnswersDTO);
                        }

                        Question newQuestion = new Question()
                        {
                            QuestionId = questionId,
                            Answers = answers,
                            Type = type,
                            ShortText = shortText,
                            Text = text,
                            ProductId = productId2,
                            Media = media,
                            LastEditDate = lastEditDate
                        };

                        newProduct.Questions.Add(newQuestion);

                        row++;
                    }

                    (bool, IResult) validResult = RequestValidator.ValidateProductModel(db, newProduct);

                    if (!validResult.Item1)
                        continue;

                    Product? targetProduct = (from p in db.Products.Include(x => x.Questions)
                                              where p.ProductId == newProduct.ProductId
                                              select p).FirstOrDefault();

                    if (targetProduct == null)
                    {
                        ProductDeleteId(db, newProduct.ProductId);
                        ProductCreate(db, newProduct);
                        continue;
                    }

                    targetProduct.Colour = newProduct.Colour;
                    targetProduct.Name = newProduct.Name;
                    targetProduct.Round = newProduct.Round;
                    targetProduct.Logo = newProduct.Logo;

                    targetProduct.Questions.Clear();

                    foreach (var question in newProduct.Questions)
                    {
                        Question newQuestion = new Question()
                        {
                            Answers = question.Answers,
                            Type = question.Type,
                            ShortText = question.ShortText,
                            Text = question.Text,
                            Media = question.Media,
                            LastEditDate = question.LastEditDate
                        };

                        newQuestion.LastEditDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Ekaterinburg Standard Time")).ToString(new CultureInfo("en-US"));
                        if (newQuestion.Type != "MEDIA") newQuestion.Media = null;

                        targetProduct.Questions.Add(newQuestion);
                    }

                    db.SaveChanges();
                }
            }

            return Results.Ok();
        }
        public IResult ProductLogoCreateId(eco_questContext db, HttpRequest request, long id)
        {
            Console.WriteLine("==========/product/logo/create/{id:long}==========");

            Product? targetProduct = (from p in db.Products
                                      where p.ProductId == id
                                      select p).FirstOrDefault();

            if (targetProduct == null)
                return Results.NotFound("Запрашиваемый продукт не найден");
            if (targetProduct.Logo != null)
                return Results.BadRequest("У запрашиваемого продукта логотип уже существует");

            IFormFile? file = request.Form.Files.FirstOrDefault();

            if (file == null)
                return Results.BadRequest("Файл не загружен");

            string filePath = $"{_app.Configuration["SourcePath"]}logo{id}{Path.GetExtension(file.FileName)}";

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            targetProduct.Logo = Path.GetFileName(filePath);

            db.SaveChanges();

            return Results.Ok();
        }
        public IResult ProductLogoDeleteId(eco_questContext db, long id)
        {
            Console.WriteLine("==========/product/logo/delete/{id:long}==========");

            Product? targetProduct = (from p in db.Products
                                      where p.ProductId == id
                                      select p).FirstOrDefault();

            if (targetProduct != null && targetProduct.Logo != null)
            {
                FileInfo? targetFile = (from f in new DirectoryInfo(_app.Configuration["SourcePath"]).GetFiles()
                                        where f.Name == Path.GetFileName(targetProduct.Logo)
                                        select f).FirstOrDefault();

                if (targetFile != null)
                    targetFile.Delete();

                targetProduct.Logo = null;
            }

            db.SaveChanges();

            return Results.Ok();
        }
        public IResult ProductLogoUpdateId(eco_questContext db, HttpRequest request, long id)
        {
            Console.WriteLine("==========/product/logo/update/{id:long}==========");

            ProductLogoDeleteId(db, id);
            return ProductLogoCreateId(db, request, id);
        }
        public IResult ProductUpdate(eco_questContext db, Product request)
        {
            Console.WriteLine("==========/product/update==========");

            (bool, IResult) validResult = RequestValidator.ValidateProductModel(db, request);

            if (!validResult.Item1)
                return validResult.Item2;

            Product? targetProduct = (from p in db.Products.Include(x => x.Questions)
                                      where p.ProductId == request.ProductId
                                      select p).FirstOrDefault();

            if (targetProduct == null)
            {
                ProductDeleteId(db, request.ProductId);
                return ProductCreate(db, request);
            }

            targetProduct.Colour = request.Colour;
            targetProduct.Name = request.Name;
            targetProduct.Round = request.Round;
            targetProduct.Logo = request.Logo;

            foreach (var question in request.Questions)
            {
                Question? targetQuestion = (from q in targetProduct.Questions
                                            where q.QuestionId == question.QuestionId
                                            select q).FirstOrDefault();

                if (targetQuestion == null)
                {
                    Question newQuestion = new Question()
                    {
                        Answers = question.Answers,
                        Type = question.Type,
                        ShortText = question.ShortText,
                        Text = question.Text,
                        Media = question.Media,
                        LastEditDate = question.LastEditDate
                    };

                    newQuestion.LastEditDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Ekaterinburg Standard Time")).ToString(new CultureInfo("en-US"));
                    if (newQuestion.Type != "MEDIA") newQuestion.Media = null;

                    targetProduct.Questions.Add(newQuestion);
                }
                else
                {
                    targetQuestion.Answers = question.Answers;
                    targetQuestion.Type = question.Type;
                    targetQuestion.ShortText = question.ShortText;
                    targetQuestion.Text = question.Text;
                    targetQuestion.LastEditDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Ekaterinburg Standard Time")).ToString(new CultureInfo("en-US"));

                    if (targetQuestion.Type != "MEDIA") targetQuestion.Media = null;
                    else targetQuestion.Media = question.Media;
                }
            }

            db.SaveChanges();

            return Results.Ok();
        }

        public IResult QuestionDeleteId(eco_questContext db, long id)
        {
            Console.WriteLine("==========/question/delete/{id:long}==========");

            Question? targetQuestion = (from q in db.Questions
                                        where q.QuestionId == id
                                        select q).FirstOrDefault();

            if (targetQuestion != null)
                db.Questions.Remove(targetQuestion);

            db.SaveChanges();

            return Results.Ok();
        }
        public IResult QuestionMediaCreateId(eco_questContext db, HttpRequest request, long id)
        {
            Console.WriteLine("==========/question/media/create/{id:long}==========");

            Question? targetQuestion = (from q in db.Questions
                                        where q.QuestionId == id
                                        select q).FirstOrDefault();

            if (targetQuestion == null)
                return Results.NotFound("Запрашиваемый вопрос не найден");
            if (targetQuestion.Type != "MEDIA")
                return Results.BadRequest("Тип запрашиваемого вопроса не является 'медиа'");
            if (targetQuestion.Media != null)
                return Results.BadRequest("У запрашиваемого вопроса медиа уже существует");

            IFormFile? file = request.Form.Files.FirstOrDefault();

            if (file == null)
                return Results.BadRequest("Файл не загружен");

            string filePath = $"{_app.Configuration["SourcePath"]}media{id}{Path.GetExtension(file.FileName)}";

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            targetQuestion.Media = Path.GetFileName(filePath);

            db.SaveChanges();

            return Results.Ok();
        }
        public IResult QuestionMediaDeleteId(eco_questContext db, long id)
        {
            Console.WriteLine("==========/question/media/delete/{id:long}==========");

            Question? targetQuestion = (from q in db.Questions
                                        where q.QuestionId == id
                                        select q).FirstOrDefault();

            if (targetQuestion != null && targetQuestion.Media != null)
            {
                FileInfo? targetFile = (from f in new DirectoryInfo(_app.Configuration["SourcePath"]).GetFiles()
                                        where f.Name == Path.GetFileName(targetQuestion.Media)
                                        select f).FirstOrDefault();

                if (targetFile != null)
                    targetFile.Delete();

                targetQuestion.Media = null;
            }

            db.SaveChanges();

            return Results.Ok();
        }
        public IResult QuestionMediaUpdateId(eco_questContext db, HttpRequest request, long id)
        {
            Console.WriteLine("==========/question/media/update/{id:long}==========");

            QuestionMediaDeleteId(db, id);
            return QuestionMediaCreateId(db, request, id);
        }

        public IResult StatisticCreate(eco_questContext db, Statistic request)
        {
            Console.WriteLine("==========/statistic/create==========");

            (bool, IResult) validResult = RequestValidator.ValidateStatisticModel(db, request);

            if (!validResult.Item1)
                return validResult.Item2;

            Statistic newRecord = new Statistic()
            {
                UserId = request.UserId,
                LastName = request.LastName,
                FirstName = request.FirstName,
                Patronymic = request.Patronymic,
                Login = request.Login,
                Date = request.Date,
                Duration = request.Duration,
                Results = request.Results
            };

            db.Statistics.Add(newRecord);

            db.SaveChanges();

            return Results.Ok();
        }
        public IResult StatisticExport(eco_questContext db, StatisticExportDTO request)
        {
            Console.WriteLine("==========/statistic/export==========");

            if (request.FileName == null || request.FileName == String.Empty)
                return Results.BadRequest("Имя файла не может иметь значение null");

            List<Statistic> allRecords = db.Statistics.ToList();
            List<(Statistic, DateTime, TimeSpan)> allRecordsDatesDurations = new List<(Statistic, DateTime, TimeSpan)>();

            foreach (var record in allRecords)
                allRecordsDatesDurations.Add((record, DateTime.Parse(record.Date, new CultureInfo("en-US"), DateTimeStyles.None), TimeSpan.Parse(record.Duration, new CultureInfo("en-US"))));

            DateTime startDate;
            DateTime endDate;
            TimeSpan startDuration;
            TimeSpan endDuration;

            bool startDateParsingResult = DateTime.TryParse(request.StartDate, new CultureInfo("en-US"), DateTimeStyles.None, out startDate);
            bool endDateParsingResult = DateTime.TryParse(request.EndDate, new CultureInfo("en-US"), DateTimeStyles.None, out endDate);
            bool startDurationParsingResult = TimeSpan.TryParse(request.StartDuration, new CultureInfo("en-US"), out startDuration);
            bool endDurationParsingResult = TimeSpan.TryParse(request.EndDuration, new CultureInfo("en-US"), out endDuration);

            if (!startDateParsingResult)
                startDate = allRecordsDatesDurations.Min(x => x.Item2);
            if (!endDateParsingResult)
                endDate = allRecordsDatesDurations.Max(x => x.Item2);
            if (!startDurationParsingResult)
                startDuration = allRecordsDatesDurations.Min(x => x.Item3);
            if (!endDurationParsingResult)
                endDuration = allRecordsDatesDurations.Max(x => x.Item3);

            if (!(startDate <= endDate))
                return Results.BadRequest("Дата начала не может быть больше даты конца");
            if (!(startDuration <= endDuration))
                return Results.BadRequest("Продолжительность начала не может быть больше продолжительности конца");

            List<Statistic> targetRecords = (from r in allRecordsDatesDurations
                                             where (r.Item2 >= startDate && r.Item2 <= endDate) && (r.Item3 >= startDuration && r.Item3 <= endDuration)
                                             orderby r.Item2, r.Item3
                                             select r.Item1).ToList();

            XLWorkbook workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add("Statistics");

            int row = 1;

            worksheet.Cell("B" + row).Value = "Дата проведения игры";
            worksheet.Cell("C" + row).Value = "Продолжительность игры";
            worksheet.Cell("D" + row).Value = "ФИО ведущего";
            worksheet.Cell("E" + row).Value = "Логин ведущего";
            worksheet.Cell("F" + row).Value = "Команда";
            worksheet.Cell("G" + row).Value = "Игрок";
            worksheet.Cell("H" + row).Value = "Очки";
            worksheet.Cell("I" + row).Value = "Место";

            worksheet.Range($"B{row}:I{row}").Style.Fill.BackgroundColor = XLColor.DarkGreen;

            worksheet.Range($"B{row}:I{row}").Style.Font.FontColor = XLColor.White;

            row++;

            StatisticsResultsDTO? statisticsResultsDTO;

            foreach (var record in targetRecords)
            {
                statisticsResultsDTO = JsonSerializer.Deserialize<StatisticsResultsDTO>(record.Results);
                if (statisticsResultsDTO == null)
                    statisticsResultsDTO = new StatisticsResultsDTO();

                foreach (var team in statisticsResultsDTO.Teams)
                {
                    foreach (var player in team.Players)
                    {
                        worksheet.Cell("A" + row).Value = record.RecordId;
                        worksheet.Cell("B" + row).Value = record.Date;
                        worksheet.Cell("C" + row).Value = record.Duration;
                        worksheet.Cell("D" + row).Value = $"{record.LastName} {record.FirstName} {record.Patronymic}";
                        worksheet.Cell("E" + row).Value = record.Login;
                        worksheet.Cell("F" + row).Value = team.Name;
                        worksheet.Cell("G" + row).Value = player;
                        worksheet.Cell("H" + row).Value = team.Score;
                        worksheet.Cell("I" + row).Value = team.Place;

                        row++;
                    }
                }
            }

            if (targetRecords.Count > 0)
            {
                worksheet.Range($"A2:A{row - 1}").Style.Fill.BackgroundColor = XLColor.LightGreen;
                worksheet.Range($"B2:I{row - 1}").Style.Fill.BackgroundColor = XLColor.Green;

                worksheet.Range($"B2:I{row - 1}").Style.Font.FontColor = XLColor.White;
            }

            worksheet.Columns().AdjustToContents();

            worksheet.Range($"A1:I{row - 1}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range($"A1:I{row - 1}").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            worksheet.Range($"A1:I{row - 1}").Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            worksheet.Range($"A1:I{row - 1}").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            List<string> oldFiles = (from file in Directory.GetFiles(_app.Configuration["SourcePath"])
                                     where Regex.IsMatch(Path.GetFileName(file), @"^statistics.*\.xlsx$")
                                     select file).ToList();

            foreach (var oldFile in oldFiles)
            {
                File.Delete(oldFile);
            }

            string filePath = $"{_app.Configuration["SourcePath"]}{request.FileName}.xlsx";

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                workbook.SaveAs(fileStream);
            }

            return Results.Ok();
        }

        public IResult UserCreate(eco_questContext db, User request)
        {
            Console.WriteLine("==========/user/create==========");

            (bool, IResult) validResult = RequestValidator.ValidateUserModel(db, request);

            if (!validResult.Item1)
                return validResult.Item2;

            User newUser = new User()
            {
                LastName = request.LastName,
                FirstName = request.FirstName,
                Patronymic = request.Patronymic,
                Login = request.Login,
                Password = PasswordHasher.Encrypt(request.Password),
                Role = request.Role,
                Status = request.Role == "player" ? "active" : "inactive"
            };

            db.Users.Add(newUser);

            db.SaveChanges();

            return Results.Ok();
        }
        public IResult UserDeleteId(eco_questContext db, long id)
        {
            Console.WriteLine("==========/user/delete/{id:long}==========");

            DeleteExpiredGames(db);

            User? targetUser = (from u in db.Users
                                where u.UserId == id
                                select u).FirstOrDefault();

            if (targetUser != null)
                db.Users.Remove(targetUser);

            db.SaveChanges();

            return Results.Ok();
        }
        public IResult UserGetActiveMasters(eco_questContext db)
        {
            Console.WriteLine("==========/user/get/activeMasters==========");

            List<User> targetUsers = (from u in db.Users
                                      where u.Role == "master" && u.Status == "active"
                                      select u).ToList();

            foreach (var user in targetUsers)
            {
                user.Password = null;
                user.Role = null;
                user.Status = null;
            }

            return Results.Json(targetUsers.OrderBy(x => x.UserId));
        }
        public IResult UserGetActivePlayers(eco_questContext db)
        {
            Console.WriteLine("==========/user/get/activePlayers==========");

            List<User> targetUsers = (from u in db.Users
                                      where u.Role == "player" && u.Status == "active"
                                      select u).ToList();

            foreach (var user in targetUsers)
            {
                user.Password = null;
                user.Role = null;
                user.Status = null;
            }

            return Results.Json(targetUsers.OrderBy(x => x.UserId));
        }
        public IResult UserGetInactiveUsers(eco_questContext db)
        {
            Console.WriteLine("==========/user/get/inactiveUsers==========");

            List<User> targetUsers = (from u in db.Users
                                      where u.Status == "inactive"
                                      select u).ToList();

            foreach (var user in targetUsers)
            {
                user.Password = null;
                user.Status = null;
            }

            return Results.Json(targetUsers.OrderBy(x => x.UserId));
        }
        public IResult UserToActiveUserId(eco_questContext db, long id)
        {
            Console.WriteLine("==========/user/toActiveUser/{id:long}==========");

            User? targetUser = (from u in db.Users
                                where u.UserId == id
                                select u).FirstOrDefault();

            if (targetUser == null)
                return Results.NotFound("Запрашиваемый пользователь не найден");
            if (targetUser.Status != "inactive")
                return Results.BadRequest("Запрашиваемый пользователь не является неактивным");

            targetUser.Status = "active";

            db.SaveChanges();

            return Results.Ok();
        }
        public IResult UserToInactiveUserId(eco_questContext db, long id)
        {
            Console.WriteLine("==========/user/toInactiveUser/{id:long}==========");

            User? targetUser = (from u in db.Users
                                where u.UserId == id
                                select u).FirstOrDefault();

            if (targetUser == null)
                return Results.NotFound("Запрашиваемый пользователь не найден");
            if (targetUser.Status != "active")
                return Results.BadRequest("Запрашиваемый пользователь не является активным");

            targetUser.Status = "inactive";

            db.SaveChanges();

            return Results.Ok();
        }
        public IResult UserUpdateInfo(eco_questContext db, User request)
        {
            Console.WriteLine("==========/user/update/info==========");

            (bool, IResult) validResult = RequestValidator.ValidateUserModel(db, request);

            if (!validResult.Item1)
                return validResult.Item2;

            User? targetUser = (from u in db.Users
                                where u.UserId == request.UserId
                                select u).FirstOrDefault();

            if (targetUser == null)
                return Results.NotFound("Запрашиваемый пользователь не найден");
            if (targetUser.Password != PasswordHasher.Encrypt(request.Password))
                return Results.BadRequest("Запрашиваемый пароль пользователя не совпадает с фактическим");

            targetUser.LastName = request.LastName;
            targetUser.FirstName = request.FirstName;
            targetUser.Patronymic = request.Patronymic;
            targetUser.Login = request.Login;

            db.SaveChanges();

            return Results.Ok();
        }
        public IResult UserUpdatePassword(eco_questContext db, UpdatePasswordDTO request)
        {
            Console.WriteLine("==========/user/update/password==========");

            (bool, IResult) validResult = RequestValidator.ValidateUpdatePasswordDTO(db, request);

            if (!validResult.Item1)
                return validResult.Item2;

            User? targetUser = (from u in db.Users
                                where u.Login == request.Login
                                select u).FirstOrDefault();

            if (targetUser == null)
                return Results.NotFound("Запрашиваемый пользователь не найден");
            if (targetUser.Password != PasswordHasher.Encrypt(request.OldPassword))
                return Results.BadRequest("Запрашиваемый пароль пользователя не совпадает с фактическим");

            targetUser.Password = PasswordHasher.Encrypt(request.NewPassword);

            db.SaveChanges();

            return Results.Ok();
        }
        public IResult UserUpdatePasswordReset(eco_questContext db, UpdatePasswordDTO request)
        {
            Console.WriteLine("==========/user/update/password/reset==========");

            if (request.Login == null || request.Login == String.Empty)
                return Results.BadRequest("Логин пользователя не может иметь значение null");
            if (request.NewPassword == null || request.NewPassword == String.Empty)
                return Results.BadRequest("Новый пароль пользователя не может иметь значение null");

            User? targetUser = (from u in db.Users
                                where u.Login == request.Login
                                select u).FirstOrDefault();

            if (targetUser == null)
                return Results.NotFound("Запрашиваемый пользователь не найден");

            targetUser.Password = PasswordHasher.Encrypt(request.NewPassword);

            db.SaveChanges();

            return Results.Ok();
        }

        public void DeleteExpiredGames(eco_questContext db)
        {
            List<Game> allGames = db.Games.ToList();
            List<(Game, DateTime)> allGamesDates = new List<(Game, DateTime)>();

            foreach (var game in allGames)
                allGamesDates.Add((game, DateTime.Parse(game.Date, new CultureInfo("en-US"), DateTimeStyles.None)));

            List<Game> targetGames = (from g in allGamesDates
                                      where DateTime.UtcNow >= g.Item2.Add(TimeSpan.FromDays(7))
                                      select g.Item1).ToList();

            db.Games.RemoveRange(targetGames);

            db.SaveChanges();
        }

        public IResult GetRandomQuiz(eco_questContext db, long id)
        {
            Quiz? targetQuiz = (from q in db.Quiz
                                where q.UserId == id
                                select q).FirstOrDefault();
            List<QuestionWeight> weights = (from q in db.QuestionWeights
                                            select q).ToList();
            if (targetQuiz != null)
                db.Quiz.Remove(targetQuiz);
            db.SaveChanges();
            var questions = db.Products.Join(db.Questions,
                p => p.ProductId,
                q => q.ProductId,
                (p, q) => new
                {
                    QuestionId = q.QuestionId,
                    ProductName = p.Name,
                    Text = q.Text,
                    Answers = q.Answers,
                    Round = p.Round,
                    Type = q.Type,
                    Media = q.Media
                }).Where(q => q.Round == 3);
            var list = questions.ToList();
            Random rand = new Random();
            HashSet<int> unique_ids = new HashSet<int>();
            while (unique_ids.Count < 15)
            {
                int first = rand.Next(list.Count);
                int second = rand.Next(list.Count);
                QuestionWeight? first_weight = (from q in weights
                                                where q.QuestionId == first
                                                select q).FirstOrDefault();
                QuestionWeight? second_weight = (from q in weights
                                                 where q.QuestionId == second
                                                 select q).FirstOrDefault();
                if (first_weight != null && second_weight != null)
                    unique_ids.Add(first_weight.Weight > second_weight.Weight ? first : second);
                else
                    unique_ids.Add(first);
            }

            List<object> result = new List<object>();
            foreach (var question_id in unique_ids)
                result.Add(list.ElementAt(question_id));

            Quiz newQuiz = new Quiz()
            {
                UserId = id,
                Duration = 0,
                CurrentQuestion = 0,
                CorrectAnswers = 0,
                Helps = "{\"Fifty\":3,\"RightMistake\":3}",
                Questions = JsonSerializer.Serialize(result, new JsonSerializerOptions()
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                })
            };

            QuizStatistic newStat = new QuizStatistic()
            {
                UserId = id,
                Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Ekaterinburg Standard Time")).ToString(new CultureInfo("en-US")),
                Mode = "random",
                UserAnswers = "[]"
            };

            db.QuizStatistics.Add(newStat);
            db.Quiz.Add(newQuiz);
            db.SaveChanges();

            return Results.Json(newQuiz);
        }

        public IResult GetQuiz(eco_questContext db, GetQuizDTO quiz)
        {
            Quiz? targetQuiz = (from q in db.Quiz
                                where q.UserId == quiz.UserId
                                select q).FirstOrDefault();
            if (targetQuiz != null)
                db.Quiz.Remove(targetQuiz);
            db.SaveChanges();
            var questions = db.Products.Join(db.Questions,
                p => p.ProductId,
                q => q.ProductId,
                (p, q) => new
                {
                    QuestionId = q.QuestionId,
                    ProductId = q.ProductId,
                    ProductName = p.Name,
                    Text = q.Text,
                    Answers = q.Answers,
                    Type = q.Type,
                    Media = q.Media
                }).Where(u => quiz.SelectedProduct.Contains(u.ProductId));
            var list = questions.ToList();
            string question_str = "";

            List<object> result = new List<object>();
            if (quiz.Mode == "challenge")
            {
                Random rand = new Random();
                HashSet<int> unique_ids = new HashSet<int>();
                while (unique_ids.Count < 15)
                    unique_ids.Add(rand.Next(list.Count));
                foreach (var question_id in unique_ids)
                    result.Add(list.ElementAt(question_id));
                question_str = JsonSerializer.Serialize(result, new JsonSerializerOptions()
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                });
            }
            if (question_str.Count() == 0)
                question_str = JsonSerializer.Serialize(questions, new JsonSerializerOptions()
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                });

            Quiz newQuiz = new Quiz()
            {
                UserId = quiz.UserId,
                Duration = 0,
                CurrentQuestion = 0,
                CorrectAnswers = 0,
                Helps = "{\"Fifty\":3,\"RightMistake\":3}",
                Questions = question_str
            };

            QuizStatistic newStat = new QuizStatistic()
            {
                UserId = quiz.UserId,
                Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Ekaterinburg Standard Time")).ToString(new CultureInfo("en-US")),
                Mode = quiz.Mode,
                UserAnswers = "[]"
            };

            db.Quiz.Add(newQuiz);
            db.QuizStatistics.Add(newStat);
            db.SaveChanges();

            return Results.Json(newQuiz);
        }

        public IResult GetChallengeQuiz(eco_questContext db, GetQuizDTO quiz)
        {
            Quiz? targetQuiz = (from q in db.Quiz
                                where q.UserId == quiz.UserId
                                select q).FirstOrDefault();
            if (targetQuiz != null)
                db.Quiz.Remove(targetQuiz);
            db.SaveChanges();

            Challenge? target_challenge = (from q in db.Challenges
                                          where q.Password == quiz.Password
                                          select q).FirstOrDefault();
            if(target_challenge == null)
                return Results.BadRequest("Такого соревнования не существует");

            var selected_questions = target_challenge.Questions.Split(',')?.Select(Int64.Parse)?.ToList();

            var questions = db.Products.Join(db.Questions,
                p => p.ProductId,
                q => q.ProductId,
                (p, q) => new
                {
                    QuestionId = q.QuestionId,
                    ProductId = q.ProductId,
                    ProductName = p.Name,
                    Text = q.Text,
                    Answers = q.Answers,
                    Type = q.Type,
                    Media = q.Media
                }).Where(u => selected_questions.Contains(u.QuestionId));
            var list = questions.ToList();
            string question_str = "";
            question_str = JsonSerializer.Serialize(questions, new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
            });

            Quiz newQuiz = new Quiz()
            {
                UserId = quiz.UserId,
                Duration = 0,
                CurrentQuestion = 0,
                CorrectAnswers = 0,
                Helps = "{\"Fifty\":3,\"RightMistake\":3}",
                Questions = question_str
            };

            QuizStatistic newStat = new QuizStatistic()
            {
                UserId = quiz.UserId,
                Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Ekaterinburg Standard Time")).ToString(new CultureInfo("en-US")),
                Mode = quiz.Mode,
                UserAnswers = "[]"
            };

            db.Quiz.Add(newQuiz);
            db.QuizStatistics.Add(newStat);
            db.SaveChanges();

            return Results.Json(newQuiz);
        }

        public IResult CheckAnswer(eco_questContext db, CheckAnswerDTO answer)
        {
            Quiz? targetQuiz = (from q in db.Quiz
                                where q.UserId == answer.UserId
                                select q).FirstOrDefault();

            QuizStatistic? targetStat = (from q in db.QuizStatistics
                                         where q.UserId == answer.UserId
                                         orderby q.Id
                                         select q).LastOrDefault();

            Question? targetQuestion = (from q in db.Questions
                                        where q.QuestionId == answer.QuestionId
                                        select q).FirstOrDefault();
            if(targetQuestion == null)
                return Results.BadRequest("Вопрос не найден");
            QuestionAnswersDTO? questionAnswersDTO = JsonSerializer.Deserialize<QuestionAnswersDTO>(targetQuestion.Answers);

            bool is_correct_answer = questionAnswersDTO.CorrectAnswers.ElementAt(0).ToLower() == answer.Answer.ToLower();
            targetQuiz.CorrectAnswers += is_correct_answer ? 1 : 0;

            List<QuizStatAnswersDTO>? stat_answers = JsonSerializer.Deserialize<List<QuizStatAnswersDTO>>(targetStat.UserAnswers);

            // Вопрос уже был добавлен в статистику.
            bool help_used = false;
            if(targetQuiz.CurrentQuestion == stat_answers.Count() - 1)
            {
                stat_answers.Last().ProductId = targetQuestion.ProductId;
                stat_answers.Last().QuestionId = answer.QuestionId;
                stat_answers.Last().Duration += answer.Duration - targetQuiz.Duration;
                stat_answers.Last().IsCorrect = is_correct_answer ? 1 : 0;
                help_used = true;
            }
            else
            {
                stat_answers.Add(new QuizStatAnswersDTO()
                {
                    QuestionId = answer.QuestionId,
                    ProductId = targetQuestion.ProductId,
                    Duration = answer.Duration - targetQuiz.Duration,
                    IsCorrect = is_correct_answer ? 1 : 0,
                    UsedHelps = 0
                });
            }
            
            targetStat.UserAnswers = JsonSerializer.Serialize(stat_answers);

            targetQuiz.Duration = answer.Duration;
            ++targetQuiz.CurrentQuestion;

            // При праве на ошибку не сохраняем в случае неверного ответа.
            if(is_correct_answer || !answer.RightMistake)
            {
                db.SaveChanges();
                long weight = 0;
                weight += is_correct_answer ? 0 : 100;
                weight += help_used ? 20 : 0;
                weight += answer.Duration < 15 ? 0 : 10;
                QuestionWeightUpdate(db, new WeightFilterDTO { ProductId = targetQuestion.ProductId,
                                                               QuestionId = answer.QuestionId,
                                                               Weight = Math.Min(weight, 100)
                });
            }

            return Results.Json(new { result = is_correct_answer, 
                                      correct_answer = questionAnswersDTO.CorrectAnswers.ElementAt(0) });
        }

        public IResult GetResult(eco_questContext db, long id)
        {
            Quiz? targetQuiz = (from q in db.Quiz
                                where q.UserId == id
                                select q).FirstOrDefault();
            var questions = JsonSerializer.Deserialize<ICollection<QuestionAnswersDTO>>(targetQuiz.Questions);

            return Results.Json(new { total_question = questions.Count(), correct_answer = targetQuiz.CorrectAnswers, total_time = targetQuiz.Duration });
        }

        public IResult UseHelp(eco_questContext db, UseHelpDTO help)
        {
            Quiz? targetQuiz = (from q in db.Quiz
                                where q.UserId == help.UserId
                                select q).FirstOrDefault();
            var helps = JsonSerializer.Deserialize<HelpsDTO>(targetQuiz.Helps);

            targetQuiz.Duration = help.Duration;
            bool is_fifty = help.Help == 1;
            if (is_fifty)
                --helps.Fifty;
            else
                --helps.RightMistake;
            var new_helps = JsonSerializer.Serialize(helps);
            targetQuiz.Helps = new_helps;


            QuizStatistic? targetStat = (from q in db.QuizStatistics
                                         where q.UserId == help.UserId
                                         orderby q.Id
                                         select q).LastOrDefault();
            List<QuizStatAnswersDTO>? stat_answers = JsonSerializer.Deserialize<List<QuizStatAnswersDTO>>(targetStat.UserAnswers);
            stat_answers.Add(new QuizStatAnswersDTO()
            {
                UsedHelps = 1
            });

            targetStat.UserAnswers = JsonSerializer.Serialize(stat_answers);

            db.SaveChanges();

            return Results.Ok();
        }

        public IResult GetPlayerStat(eco_questContext db, PlayerStatFilterDTO filter)
        {
            List<object> res = new List<object>();
            List<User> players = (from q in db.Users
                                  where q.Role == "player"
                                  select q).ToList();
            if (filter.UserId != -1)
                players = players.Where(p => p.UserId == filter.UserId).ToList();
            foreach (User player in players)
            {
                List<QuizStatistic> users_stat = (from q in db.QuizStatistics
                                                  where q.UserId == player.UserId
                                                  select q).ToList();
                if (filter.Mode != "common")
                    users_stat = users_stat.Where(p => p.Mode == filter.Mode).ToList();
                if (filter.Interval != "all")
                {
                    var current_date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Ekaterinburg Standard Time"));
                    var end_date = current_date.AddDays(filter.Interval == "week" ? -7 : filter.Interval == "month" ? -30 : -365);
                    users_stat = users_stat.Where(p =>  DateTime.Parse(p.Date, new CultureInfo("en-US"), DateTimeStyles.None) <= current_date && 
                                                        DateTime.Parse(p.Date, new CultureInfo("en-US"), DateTimeStyles.None) >= end_date).ToList();
                }

                long total_correct_answer = 0;
                long total_use_help = 0;
                long total_duration = 0;
                long total_answer = 0;
                foreach(QuizStatistic stat in users_stat)
                {
                    List<QuizStatAnswersDTO>? stat_answers = JsonSerializer.Deserialize<List<QuizStatAnswersDTO>>(stat.UserAnswers);
                    foreach(QuizStatAnswersDTO answer in stat_answers)
                    {
                        if (filter.ProductId != -1 && answer.ProductId != filter.ProductId)
                            continue;
                        ++total_answer;
                        total_correct_answer += answer.IsCorrect;
                        total_use_help += answer.UsedHelps;
                        total_duration += answer.Duration;
                    }
                }
                if (total_answer == 0)
                    continue;
                res.Add(new { user_id = player.UserId, name = player.LastName + " " + player.FirstName.ElementAt(0) + "." + player.Patronymic.ElementAt(0) + ".", 
                              percent_correct = (float)total_correct_answer / total_answer * 100, 
                              percent_help = (float)total_use_help / total_answer * 100, badges = 0, total_quiz = users_stat.Count(), 
                              duration = total_duration / total_answer});
            }
            return Results.Json(res);
        }

        public IResult GetChartData(eco_questContext db, PlayerStatFilterDTO filter)
        {
            List<ChartDataDTO> res = new List<ChartDataDTO>();
            List<QuizStatistic> users_stat = (from q in db.QuizStatistics
                                              where q.UserId == filter.UserId
                                              select q).ToList();
            var query = from q in db.Questions
                        group q by q.ProductId into qGroup
                        select new
                        {
                            ProductId = qGroup.Key,
                            Count = qGroup.Count(),
                        };
            var questions_by_product_id = query.ToDictionary(x => x.ProductId, x => x.Count);
            if (filter.Mode != "common")
                users_stat = users_stat.Where(p => p.Mode == filter.Mode).ToList();
            if (filter.Interval != "all")
            {
                var current_date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Ekaterinburg Standard Time"));
                var end_date = current_date.AddDays(filter.Interval == "week" ? -7 : filter.Interval == "month" ? -30 : -365);
                users_stat = users_stat.Where(p => DateTime.Parse(p.Date, new CultureInfo("en-US"), DateTimeStyles.None) <= current_date &&
                                                   DateTime.Parse(p.Date, new CultureInfo("en-US"), DateTimeStyles.None) >= end_date).ToList();
            }

            Dictionary<long, ChartDataDTO> stat_product_by_id = new Dictionary<long, ChartDataDTO>();
            Dictionary<long, HashSet<long>> unique_answer_by_product_id = new Dictionary<long, HashSet<long>>();
            foreach (QuizStatistic stat in users_stat)
            {
                List<QuizStatAnswersDTO>? stat_answers = JsonSerializer.Deserialize<List<QuizStatAnswersDTO>>(stat.UserAnswers);
                foreach (QuizStatAnswersDTO answer in stat_answers)
                {
                    if(!stat_product_by_id.ContainsKey(answer.ProductId))
                    {
                        stat_product_by_id.Add(answer.ProductId, new ChartDataDTO());
                        unique_answer_by_product_id.Add(answer.ProductId, new HashSet<long>());
                    }
                    ++stat_product_by_id[answer.ProductId].TotalQuiz;
                    stat_product_by_id[answer.ProductId].PercentCorrect += answer.IsCorrect;
                    stat_product_by_id[answer.ProductId].PercentHelp += answer.UsedHelps;
                    stat_product_by_id[answer.ProductId].Duration += answer.Duration;
                    unique_answer_by_product_id[answer.ProductId].Add(answer.QuestionId);
                }
            }

            foreach (KeyValuePair<long, ChartDataDTO> entry in stat_product_by_id)
            {
                entry.Value.PercentCorrect = (int)((float)entry.Value.PercentCorrect / entry.Value.TotalQuiz * 100);
                entry.Value.PercentHelp = (int)((float)entry.Value.PercentHelp / entry.Value.TotalQuiz * 100);
                entry.Value.Duration = (int)((float)entry.Value.Duration / entry.Value.TotalQuiz);
                entry.Value.ProductId = entry.Key;
                entry.Value.UniqueAnswers = (int)((float)unique_answer_by_product_id[entry.Key].Count / questions_by_product_id[entry.Key] * 100);
                res.Add(entry.Value);
            }

            return Results.Json(res);
        }

        public async void QuestionWeightUpdate(eco_questContext db, WeightFilterDTO filter)
        {
            QuestionWeight? weight = (from q in db.QuestionWeights
                                      where (filter.QuestionId == null || q.QuestionId == filter.QuestionId) &&
                                            (filter.ProductId == null || q.ProductId == filter.ProductId)
                                      select q).FirstOrDefault();
            if(weight == null)
            {
                db.QuestionWeights.Add(new QuestionWeight()
                {
                    ProductId = (long)filter.ProductId,
                    QuestionId = (long)filter.QuestionId,
                    Weight = (long)filter.Weight
                });
            }
            else
                weight.Weight = (weight.Weight + (long)filter.Weight) / 2;

            db.SaveChanges();
        }

        public IResult GetQuestionWeight(eco_questContext db, WeightFilterDTO filter)
        {
            if (filter.QuestionId == null && filter.ProductId == null)
                return Results.BadRequest("Невозможно найти вес вопроса по заданному фильтру");
            long weight = 0;
            if (filter.QuestionId != null)
            {
                QuestionWeight? weight_obj = (from q in db.QuestionWeights
                                              where q.QuestionId == filter.QuestionId
                                              select q).FirstOrDefault();
                if(weight_obj != null)
                    weight = weight_obj.Weight;
                else
                {
                    RelationQuestion? relation = (from q in db.RelationsQuestion
                                                 where q.FirstQuestion == filter.QuestionId
                                                 select q).FirstOrDefault();
                    if (relation != null)
                    {
                        weight_obj = (from q in db.QuestionWeights
                                   where q.QuestionId == relation.SecondQuestion
                                   select q).FirstOrDefault();
                        if (weight_obj != null)
                            weight = weight_obj.Weight;
                    }
                }
            }
            else if(filter.ProductId != null)
            {
                List<QuestionWeight> weights = (from q in db.QuestionWeights
                                                where q.ProductId == filter.ProductId
                                                select q).ToList();
                if(weights.Count > 0)
                {
                    foreach (QuestionWeight weight_obj in weights)
                        weight += weight_obj.Weight;
                    weight /= weights.Count;
                }
                else
                {
                    RelationProduct? relation = (from q in db.RelationsProduct
                                                 where q.FirstProduct == filter.ProductId
                                                 select q).FirstOrDefault();
                    if(relation != null)
                    {
                        weights = (from q in db.QuestionWeights
                                   where q.ProductId == relation.SecondProduct
                                   select q).ToList();
                        if (weights.Count > 0)
                        {
                            foreach (QuestionWeight weight_obj in weights)
                                weight += weight_obj.Weight;
                            weight /= weights.Count;
                        }
                    }
                }
            }
            return Results.Json(weight);
        }

        public IResult GetQuestionWeightToPlayer(eco_questContext db, long id)
        {
            List<object> res = new List<object>();
            List<QuizStatistic> statistic_records = (from q in db.QuizStatistics
                                                     where q.UserId == id
                                                     select q).ToList();
            Dictionary<long, long> weights_by_id = new Dictionary<long, long>();
            Dictionary<long, long> answers_by_id = new Dictionary<long, long>();
            foreach (QuizStatistic stat in statistic_records)
            {
                List<QuizStatAnswersDTO>? stat_answers = JsonSerializer.Deserialize<List<QuizStatAnswersDTO>>(stat.UserAnswers);
                foreach (QuizStatAnswersDTO answer in stat_answers)
                {
                    if (!weights_by_id.ContainsKey(answer.ProductId))
                    {
                        weights_by_id.Add(answer.ProductId, Math.Min(100 * (1 - answer.IsCorrect) + 20 * answer.UsedHelps + 10 * (answer.Duration > 15 ? 1 : 0), 100));
                        answers_by_id.Add(answer.ProductId, 1);
                        continue;
                    }
                    weights_by_id[answer.ProductId] += Math.Min(100 * (1 - answer.IsCorrect) + 20 * answer.UsedHelps + 10 * (answer.Duration > 15 ? 1 : 0), 100);
                    ++answers_by_id[answer.ProductId];
                }
            }
            foreach (KeyValuePair<long, long> entry in weights_by_id)
            {
                res.Add(
                        new {
                            ProductId = entry.Key,
                            Weight = entry.Value / answers_by_id[entry.Key]
                        });
            }
            return Results.Json(res);
        }

        public IResult GetQuestionWeightAll(eco_questContext db)
        {
            List<object> result = new List<object>();
            List<Question> questions = (from q in db.Questions
                                       select q).ToList();
            foreach(Question question in questions)
            {
                long weight = 0;
                QuestionWeight? weight_obj = (from q in db.QuestionWeights
                                              where q.QuestionId == question.QuestionId
                                              select q).FirstOrDefault();
                if (weight_obj != null)
                    weight = weight_obj.Weight;
                else
                {
                    RelationQuestion? relation = (from q in db.RelationsQuestion
                                                  where q.FirstQuestion == question.QuestionId
                                                  select q).FirstOrDefault();
                    if (relation != null)
                    {
                        weight_obj = (from q in db.QuestionWeights
                                      where q.QuestionId == relation.SecondQuestion
                                      select q).FirstOrDefault();
                        if (weight_obj != null)
                            weight = weight_obj.Weight;
                    }
                }
                result.Add(new {QuestionId = question.QuestionId, Weight = weight });
            }
            return Results.Json(result);
        }

        public IResult UpdateRelation(eco_questContext db, UpdateRelationDTO filter)
        {
            RelationProduct? relation = (from q in db.RelationsProduct
                                         where q.SecondProduct == filter.SecondProduct
                                         select q).FirstOrDefault();
            if(filter.FirstProduct == null && relation == null)
                return Results.Ok();

            if (relation != null)
            {
                db.Remove(relation);
                if(filter.FirstProduct != null)
                {
                    db.Add(new RelationProduct()
                    {
                        FirstProduct = (long)filter.FirstProduct,
                        SecondProduct = filter.SecondProduct
                    });
                }
            }
            else
                db.Add(new RelationProduct()
                {
                    FirstProduct = (long)filter.FirstProduct,
                    SecondProduct = filter.SecondProduct
                });
            db.SaveChanges();
            return Results.Ok();
        }

        public IResult GetRelation(eco_questContext db, long id)
        {
            RelationProduct? relation = (from q in db.RelationsProduct
                                         where q.SecondProduct == id
                                         select q).FirstOrDefault();
            if(relation == null)
                return Results.Json(new { Id = -1, Name = "Не выбрано" });
            Product? product = (from q in db.Products
                                where q.ProductId == relation.FirstProduct
                                select q).FirstOrDefault();
            return Results.Json(new { Id = relation.FirstProduct, Name = product.Name});
        }

        public IResult GetQuestionRelation(eco_questContext db, long id)
        {
            RelationQuestion? relation = (from q in db.RelationsQuestion
                                         where q.SecondQuestion == id
                                         select q).FirstOrDefault();
            if (relation == null)
                return Results.Json(new { Id = -1, ShortText = "Не выбрано" });
            Question? question = (from q in db.Questions
                                  where q.QuestionId == relation.FirstQuestion
                                  select q).FirstOrDefault();
            return Results.Json(new { Id = relation.FirstQuestion, ShortText = question.ShortText });
        }

        public IResult StatisticPlayersExport(eco_questContext db, QuizStatisticExportDTO request)
        {
            List<QuizStatistic> statistic_records = (from q in db.QuizStatistics
                                                     orderby q.Id
                                                     select q).ToList();
            List<User> players = (from q in db.Users
                                  where q.Role == "player"
                                  select q).ToList();
            var question_query = from q in db.Questions
                        select new
                        {
                            QuestionId = q.QuestionId,
                            Text = q.Text,
                        };
            var question_by_id = question_query.ToDictionary(x => x.QuestionId, x => x.Text);
            var product_query = from q in db.Products
                        select new
                        {
                            ProductId = q.ProductId,
                            Name = q.Name,
                        };
            var product_name_by_id = product_query.ToDictionary(x => x.ProductId, x => x.Name);
            XLWorkbook workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add("QuizStatistics");

            Dictionary<string, string> mode_dict = new Dictionary<string, string>();
            mode_dict.Add("train", "Тренировочный режим");
            mode_dict.Add("random", "Случайный режим");
            mode_dict.Add("challenge", "Соревновательный режим");

            int row = 1;
            worksheet.Cell("B" + row).Value = "Дата проведения викторины";
            worksheet.Cell("C" + row).Value = "ФИО игрока";
            worksheet.Cell("D" + row).Value = "Режим";
            worksheet.Cell("E" + row).Value = "Продукт";
            worksheet.Cell("F" + row).Value = "Вопрос";
            worksheet.Cell("G" + row).Value = "Верный ответ";
            worksheet.Cell("H" + row).Value = "Использована подсказка";

            foreach (var record in statistic_records)
            {
                int start_row = row;
                User? player = (from q in players
                                where q.UserId == record.UserId
                                select q).FirstOrDefault();
                List<QuizStatAnswersDTO>? stat_answers = JsonSerializer.Deserialize<List<QuizStatAnswersDTO>>(record.UserAnswers);
                foreach (QuizStatAnswersDTO answer in stat_answers)
                {
                    row++;
                    worksheet.Cell("A" + row).Value = record.Id.ToString();
                    worksheet.Cell("B" + row).Value = record.Date;
                    if (player != null)
                        worksheet.Cell("C" + row).Value = $"{player.LastName} {player.FirstName} {player.Patronymic}";
                    worksheet.Cell("D" + row).Value = mode_dict[record.Mode];
                    worksheet.Cell("E" + row).Value = product_name_by_id[answer.ProductId];
                    worksheet.Cell("F" + row).Value = question_by_id[answer.QuestionId];
                    worksheet.Cell("G" + row).Value = answer.IsCorrect == 1 ? "Да" : "Нет";
                    worksheet.Cell("H" + row).Value = answer.UsedHelps == 1 ? "Да" : "Нет";
                }
            }
            worksheet.Columns().AdjustToContents();
            worksheet.Range($"A1:H{row}").Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            worksheet.Range($"A1:H{row}").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Range($"A1:H{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range($"A1:H{row}").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            List<string> oldFiles = (from file in Directory.GetFiles(_app.Configuration["SourcePath"])
                                     where Regex.IsMatch(Path.GetFileName(file), @"^quiz_statistics.*\.xlsx$")
                                     select file).ToList();

            foreach (var oldFile in oldFiles)
            {
                File.Delete(oldFile);
            }

            string filePath = $"{_app.Configuration["SourcePath"]}{request.FileName}.xlsx";

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                workbook.SaveAs(fileStream);
            }
            return Results.Ok();
        }

        public IResult GetAllChallenge(eco_questContext db)
        {
            List<Challenge> challenges = (from q in db.Challenges
                                          orderby q.ChallengeId
                                          select q).ToList();
            foreach (var challenge in challenges)
                challenge.Questions = null;
            return Results.Json(challenges);
        }

        public IResult GetChallengeById(eco_questContext db, long id)
        {
            Challenge? challenge = (from q in db.Challenges
                                   where q.ChallengeId == id
                                   select q).FirstOrDefault();
            return Results.Json(challenge);
        }

        public IResult DeleteChallengeById(eco_questContext db, long id)
        {
            Challenge? challenge = (from q in db.Challenges
                                    where q.ChallengeId == id
                                    select q).FirstOrDefault();
            if (challenge != null)
                db.Challenges.Remove(challenge);
            db.SaveChanges();
            return Results.Ok();
        }

        public IResult UpdateChallenge(eco_questContext db, Challenge input_challenge)
        {
            Challenge? challenge = (from q in db.Challenges
                                    where q.ChallengeId == input_challenge.ChallengeId
                                    select q).FirstOrDefault();
            if (challenge != null)
            {
                challenge.Name = input_challenge.Name;
                challenge.Password = input_challenge.Password;
                challenge.Questions = input_challenge.Questions;
            }
            else
            {
                db.Challenges.Add(new Challenge
                {
                    Name = input_challenge.Name,
                    Password = input_challenge.Password,
                    Questions = input_challenge.Questions,
                });
            }
            db.SaveChanges();
            return Results.Ok();
        }

        public IResult CheckChallengeExist(eco_questContext db, Challenge input_challenge)
        {
            Challenge? challenge = (from q in db.Challenges
                                    where q.Password == input_challenge.Password
                                    select q).FirstOrDefault();
            if(challenge != null)
                return Results.Ok();
            else
                return Results.BadRequest("Соревнование с таким паролем отсутствует!");
        }
    }
}