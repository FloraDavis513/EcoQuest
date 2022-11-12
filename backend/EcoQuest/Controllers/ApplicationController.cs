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
            _app.MapGet("/", [Authorize(Roles = "adminadmin, masteractive, masterinactive, player")] () =>
            {
                Console.WriteLine("==========/==========");
                return Results.Ok();
            });
            _app.MapGet("/auth/login", [Authorize(Roles = "adminadmin, masteractive, masterinactive, player")] () =>
            {
                Console.WriteLine("==========/auth/login==========");
                return Results.Ok();
            });
            _app.MapGet("/auth/registration", [Authorize(Roles = "adminadmin, masteractive, masterinactive, player")] () =>
            {
                Console.WriteLine("==========/auth/registration==========");
                return Results.Ok();
            });
            _app.MapGet("/fields", [Authorize(Roles = "adminadmin")] () =>
            {
                Console.WriteLine("==========/fields==========");
                return Results.Ok();
            });
            _app.MapGet("/game", [Authorize(Roles = "adminadmin, masteractive, player")] () =>
            {
                Console.WriteLine("==========/game==========");
                return Results.Ok();
            });
            _app.MapGet("/lobby", [Authorize(Roles = "adminadmin, masteractive, player")] () =>
            {
                Console.WriteLine("==========/lobby==========");
                return Results.Ok();
            });
            _app.MapGet("/status", [Authorize(Roles = "adminadmin, masteractive")] () =>
            {
                Console.WriteLine("==========/status==========");
                return Results.Ok();
            });
            _app.MapGet("/templates", [Authorize(Roles = "adminadmin, masteractive")] () =>
            {
                Console.WriteLine("==========/templates==========");
                return Results.Ok();
            });

            _app.MapPost("/authentication/login/master", _service.AuthenticationLoginMaster);
            _app.MapPost("/authentication/login/player", _service.AuthenticationLoginPlayer);

            _app.MapPost("/board/create", _service.BoardCreate);
            _app.MapDelete("/board/delete/{id:long}", _service.BoardDeleteId);
            _app.MapGet("/board/get/{id:long}", _service.BoardGetId);
            _app.MapGet("/board/getAll", _service.BoardGetAll);
            _app.MapPost("/board/update", _service.BoardUpdate);

            _app.MapGet("/game/getAnswer", _service.GameGetAnswer);
            _app.MapPost("/game/setQuestion", _service.GameSetQuestion);

            _app.MapPost("/product/create", _service.ProductCreate);
            _app.MapDelete("/product/delete/{id:long}", _service.ProductDeleteId);
            _app.MapGet("/product/getAll", _service.ProductGetAll);
            _app.MapGet("/product/getAll/{round:int}", _service.ProductGetAllRound);
            _app.MapPost("/product/update", _service.ProductUpdate);

            _app.MapDelete("/question/delete/{id:long}", _service.QuestionDeleteId);

            _app.MapPost("/user/create", _service.UserCreate);
            _app.MapGet("/user/delete/{id:long}", _service.UserDeleteId);
            _app.MapGet("/user/getActiveMasters", _service.UserGetActiveMasters);
            _app.MapGet("/user/getInactiveMasters", _service.UserGetInactiveMasters);
            _app.MapGet("/user/toActiveMaster/{id:long}", _service.UserToActiveMasterId);
        }
    }
}