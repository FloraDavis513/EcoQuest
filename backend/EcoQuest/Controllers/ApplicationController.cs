using Microsoft.AspNetCore.Authorization;

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

            _app.MapPost("/authentication/login/master", _service.AuthenticationLoginMaster);
            _app.MapPost("/authentication/login/player", _service.AuthenticationLoginPlayer);

            _app.MapPost("/game/create", _service.GameCreate);
            _app.MapDelete("/game/delete/{id:long}", _service.GameDeleteId);
            _app.MapGet("/game/get/{id:long}", _service.GameGetId);
            _app.MapGet("/game/get/all", _service.GameGetAll);
            _app.MapGet("/game/get/all/{id:long}", _service.GameGetAllId);
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

            _app.MapDelete("/question/delete/{id:long}", _service.QuestionDeleteId);
            _app.MapPost("/question/media/create/{id:long}", _service.QuestionMediaCreateId);
            _app.MapDelete("/question/media/delete/{id:long}", _service.QuestionMediaDeleteId);
            _app.MapPost("/question/media/update/{id:long}", _service.QuestionMediaUpdateId);

            _app.MapPost("/statistic/create", _service.StatisticCreate);
            _app.MapPost("/statistic/export", _service.StatisticExport);

            _app.MapPost("/user/create", _service.UserCreate);
            _app.MapDelete("/user/delete/{id:long}", _service.UserDeleteId);
            _app.MapGet("/user/get/activeMasters", _service.UserGetActiveMasters);
            _app.MapGet("/user/get/inactiveMasters", _service.UserGetInactiveMasters);
            _app.MapPost("/user/toActiveMaster/{id:long}", _service.UserToActiveMasterId);
            _app.MapPost("/user/toInactiveMaster/{id:long}", _service.UserToInactiveMasterId);
            _app.MapPost("/user/update/info", _service.UserUpdateInfo);
            _app.MapPost("/user/update/password", _service.UserUpdatePassword);
        }
    }
}