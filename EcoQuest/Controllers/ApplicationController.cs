﻿using Microsoft.AspNetCore.Authorization;

namespace EcoQuest
{
    public class ApplicationController
    {
        public ApplicationController(WebApplication app, ApplicationService service)
        {
            _app = app;
            _service = service;
        }

        private readonly WebApplication _app;
        private readonly ApplicationService _service;

        public void Map()
        {
            _app.MapGet("/", [Authorize(Roles = "adminactive, masteractive, masterinactive, player")] () =>
            {
                Console.WriteLine("==========/==========");
                return Results.Ok();
            });
            _app.MapGet("/auth/login", [Authorize(Roles = "adminactive, masteractive, masterinactive, player")] () =>
            {
                Console.WriteLine("==========/auth/login==========");
                return Results.Ok();
            });
            _app.MapGet("/auth/registration", [Authorize(Roles = "adminactive, masteractive, masterinactive, player")] () =>
            {
                Console.WriteLine("==========/auth/registration==========");
                return Results.Ok();
            });
            _app.MapGet("/fields", [Authorize(Roles = "adminactive")] () =>
            {
                Console.WriteLine("==========/fields==========");
                return Results.Ok();
            });
            _app.MapGet("/game", [Authorize(Roles = "adminactive, masteractive, player")] () =>
            {
                Console.WriteLine("==========/game==========");
                return Results.Ok();
            });
            _app.MapGet("/lobby", [Authorize(Roles = "adminactive, masteractive, player")] () =>
            {
                Console.WriteLine("==========/lobby==========");
                return Results.Ok();
            });
            _app.MapGet("/status", [Authorize(Roles = "adminactive, masteractive")] () =>
            {
                Console.WriteLine("==========/status==========");
                return Results.Ok();
            });
            _app.MapGet("/templates", [Authorize(Roles = "adminactive, masteractive")] () =>
            {
                Console.WriteLine("==========/templates==========");
                return Results.Ok();
            });
            _app.MapGet("/gamer", [Authorize(Roles = "playeractive")] () =>
            {
                Console.WriteLine("==========/gamer==========");
                return Results.Ok();
            });
            _app.MapGet("/quiz", [Authorize(Roles = "playeractive")] () =>
            {
                Console.WriteLine("==========/quiz==========");
                return Results.Ok();
            });

            _app.MapPost("/authentication/login/master", _service.AuthenticationLoginMaster);
            _app.MapPost("/authentication/login/player", _service.AuthenticationLoginPlayer);

            _app.MapPost("/game/create", _service.GameCreate);
            _app.MapDelete("/game/delete/{id:long}", _service.GameDeleteId);
            _app.MapGet("/game/get/{id:long}", _service.GameGetId);
            _app.MapGet("/game/get/all", _service.GameGetAll);
            _app.MapGet("/game/get/all/{id:long}", _service.GameGetAllId);
            _app.MapPost("/game/state/players/create/{id:long}", _service.GameStatePlayersCreateId);
            _app.MapDelete("/game/state/players/delete/{gameId:long}/{playerId:long}", _service.GameStatePlayersDeleteGameIdPlayerId);
            _app.MapPost("/game/state/players/update/{id:long}", _service.GameStatePlayersUpdateId);
            _app.MapPost("/game/update", _service.GameUpdate);
            _app.MapPost("/game/update/stateAndQuestion", _service.GameUpdateStateAndQuestion);

            _app.MapPost("/gameBoard/create", _service.GameBoardCreate);
            _app.MapDelete("/gameBoard/delete/{id:long}", _service.GameBoardDeleteId);
            _app.MapGet("/gameBoard/get/{id:long}", _service.GameBoardGetId);
            _app.MapGet("/gameBoard/get/all", _service.GameBoardGetAll);
            _app.MapGet("/gameBoard/get/all/{id:long}", _service.GameBoardGetAllId);
            _app.MapPost("/gameBoard/share/{fromUserId:long}/{gameBoardId:long}/{toUserId:long}", _service.GameBoardShareFromUserIdGameBoardIdToUserId);
            _app.MapPost("/gameBoard/update", _service.GameBoardUpdate);

            _app.MapPost("/product/create", _service.ProductCreate);
            _app.MapPost("/product/export", _service.ProductExport);
            _app.MapDelete("/product/delete/{id:long}", _service.ProductDeleteId);
            _app.MapGet("/product/get/all", _service.ProductGetAll);
            _app.MapGet("/product/get/all/{round:int}", _service.ProductGetAllRound);
            _app.MapPost("/product/import", _service.ProductImport);
            _app.MapPost("/product/logo/create/{id:long}", _service.ProductLogoCreateId);
            _app.MapDelete("/product/logo/delete/{id:long}", _service.ProductLogoDeleteId);
            _app.MapPost("/product/logo/update/{id:long}", _service.ProductLogoUpdateId);
            _app.MapPost("/product/update", _service.ProductUpdate);
            _app.MapPost("/product/relation/update", _service.UpdateRelation);
            _app.MapGet("/product/relation/get/{id:long}", _service.GetRelation);

            _app.MapDelete("/question/delete/{id:long}", _service.QuestionDeleteId);
            _app.MapPost("/question/media/create/{id:long}", _service.QuestionMediaCreateId);
            _app.MapDelete("/question/media/delete/{id:long}", _service.QuestionMediaDeleteId);
            _app.MapPost("/question/media/update/{id:long}", _service.QuestionMediaUpdateId);
            _app.MapPost("/question/weight/get", _service.GetQuestionWeight);
            _app.MapGet("/question/weight/get/all/{id:long}", _service.GetQuestionWeightToPlayer);
            _app.MapGet("/question/weight/get/all", _service.GetQuestionWeightAll);
            _app.MapGet("/question/relation/get/{id:long}", _service.GetQuestionRelation);

            _app.MapPost("/statistic/create", _service.StatisticCreate);
            _app.MapPost("/statistic/export", _service.StatisticExport);
            _app.MapPost("/statistic/user", _service.GetPlayerStat);
            _app.MapPost("/statistic/chart", _service.GetChartData);
            _app.MapPost("/statistic/players/export", _service.StatisticPlayersExport);

            _app.MapPost("/user/create", _service.UserCreate);
            _app.MapDelete("/user/delete/{id:long}", _service.UserDeleteId);
            _app.MapGet("/user/get/activeMasters", _service.UserGetActiveMasters);
            _app.MapGet("/user/get/activePlayers", _service.UserGetActivePlayers);
            _app.MapGet("/user/get/inactiveUsers", _service.UserGetInactiveUsers);
            _app.MapPost("/user/toActiveUser/{id:long}", _service.UserToActiveUserId);
            _app.MapPost("/user/toInactiveUser/{id:long}", _service.UserToInactiveUserId);
            _app.MapPost("/user/update/info", _service.UserUpdateInfo);
            _app.MapPost("/user/update/password", _service.UserUpdatePassword);
            _app.MapPost("/user/update/password/reset", _service.UserUpdatePasswordReset);

            _app.MapGet("/quiz/get/random/{id:long}", _service.GetRandomQuiz);
            _app.MapPost("/quiz/get", _service.GetQuiz);
            _app.MapPost("/quiz/get/challenge", _service.GetChallengeQuiz);
            _app.MapPost("/quiz/checkAnswer", _service.CheckAnswer);
            _app.MapGet("/quiz/result/{id:long}", _service.GetResult);
            _app.MapPost("/quiz/help", _service.UseHelp);

            _app.MapGet("/challenge/get/all", _service.GetAllChallenge);
            _app.MapGet("/challenge/get/{id:long}", _service.GetChallengeById);
            _app.MapPost("/challenge/check", _service.CheckChallengeExist);
            _app.MapPost("/challenge/update", _service.UpdateChallenge);
            _app.MapDelete("/challenge/delete/{id:long}", _service.DeleteChallengeById);
        }
    }
}