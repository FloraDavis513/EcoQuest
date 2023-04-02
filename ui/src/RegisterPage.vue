<template>
  <div class="limiter">
      <div class="container-login100">
          <div class="wrap-login100 col-sm-3 mx-auto">             
                  <div class="login100-form-title">
                      <h1>СберКвест</h1>
                  </div>

                  <div class="login100-">
                      <div class="form-group">
                          <label for="name">ФИО</label>
                          <input type="text" class="form-control" id="name" placeholder="Введите имя">

                      </div>

                      <div class="form-group">
                          <label for="login">Логин</label>
                          <input type="email" class="form-control" id="login" placeholder="Введите логин">

                      </div>

                      <div class="form-group">
                          <label for="password">Пароль</label>
                          <input type="password" class="form-control" id="password" placeholder="Введите пароль">
                      </div>

                      <div class="form-group">
                          <label for="repeatPassword">Повторите пароль</label>
                          <input type="password" class="form-control" id="repeatPassword" placeholder="Введите пароль">
                      </div>


                      <div class="form-group">
                          <button type="submit" class="login_button">Зарегистрироваться</button>
                      </div>

                      <div class="form-group">
                          <button type="submit" class="alreadyLogin_button">У меня уже есть аккаунт</button>
                      </div>
                  </div>                             
          </div>      
      </div>  
  </div>
</template>

<script>


export default {
  
}
</script>

<style>
    .login100-form {
        width: 100%;
    }

    .login100-form-title {
        display: block;
        font-size: 30px;
        color: #333333;
        line-height: 1.2;
        text-align: center;
        padding: 10px 20px;
    }
    .login_button {
        margin: 0 auto;
        display: block;
        background: rgb(0 211 27);
        border: 0;
        padding: 10px 20px;
        color: white;
        border-radius: 20px;
        outline: 1px solid groove;
    }
    

    .alreadyLogin_button {
        margin: 0 auto;
        display: block;
        background: white;
        border-color: rgb(0 211 27);
        border: 1px solid rgb(0 211 27);
        padding: 10px 20px;
        color: rgb(4 182 7);
        border-radius: 20px;
    }

    .wrap-login100 {
        width: 300px;
        background: #fff;
        border-radius: 10px;
        overflow: hidden;
        padding: 55px 55px 33px 55px;
        box-shadow: 0 5px 10px 0px rgba(0, 0, 0, 0.1);
        -moz-box-shadow: 0 5px 10px 0px rgba(0, 0, 0, 0.1);
        -webkit-box-shadow: 0 5px 10px 0px rgba(0, 0, 0, 0.1);
        -o-box-shadow: 0 5px 10px 0px rgba(0, 0, 0, 0.1);
        -ms-box-shadow: 0 5px 10px 0px rgba(0, 0, 0, 0.1);
    }
    .container-login100 {
        width: 100%;
        min-height: 100vh;
        display: -webkit-box;
        display: -webkit-flex;
        display: -moz-box;
        display: -ms-flexbox;
        display: flex;
        flex-wrap: wrap;
        justify-content: center;
        align-items: center;
        padding: 15px;
        background: #f2f2f2;
    }

    .limiter {
        width: 100%;
        margin: 0 auto;
    }

</style>
