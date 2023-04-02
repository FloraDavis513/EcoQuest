<template>
<div class="Auth">
  <div class="AuthLogo">
      СберКвест
  </div>
  <div class="group_auth" >
    <div class="AuthHeader">
      Вход
    </div>
    <div id="group_tabs">
        <div class="tab" id="tab_1" @click="select_tab_1">
            Войти по логину
        </div>
        <div class="tab" id="tab_2" @click="select_tab_2">
            Войти по id комнаты
        </div>
    </div>
    <form @submit.prevent="log_in">
      <div v-if="user_type == 'Master'" id="login_master">
        <input class="form-group" id="email_master" name="username" placeholder="Логин/E-mail">
        <input type="password" class="form-group" id="password" name="password" placeholder="Пароль">
        <div :style="'visibility:' + check_failed()" id="failed_message_master">Неверный логин или пароль</div>
        <input type="submit" id="enter" value="Войти">
        <div id="get_pass">
          Забыли пароль?
        </div>
        <div id="registration">
          <div id="reg_text">
            У вас ещё нет аккаунта?
          </div>
          <div id="go_to_reg">
            Зарегистрируйтесь
          </div>
        </div>
      </div>
    </form>
    <div v-if="user_type == 'User'" id="login_user">
        <input class="form-group" placeholder="id комнаты">
        <input class="form-group" placeholder="Ваше имя">
        <div :style="'visibility:' + check_failed()" id="failed_message_user">Неверный id комнаты</div>
        <div id="enter" @click="test_action">
            Войти
        </div>
        <div id="user_message">
                Для входа запросите id комнаты у своего ведущего
        </div>
    </div>
    <!-- <div v-if="user_type == 'Master'" id="login_master">
        <input class="form-group" id="email_master" placeholder="Логин/E-mail">
        <input type="password" class="form-group" id="password" placeholder="Пароль">
        <div :style="'visibility:' + check_failed()" id="failed_message_master">Неверный логин или пароль</div>
        <div id="enter" @click="log_in">
            Войти
        </div>
        <div id="get_pass">
                Забыли пароль?
        </div>
        <div id="registration">
            <div id="reg_text">
            У вас ещё нет аккаунта?
            </div>
            <div id="go_to_reg">
            Зарегистрируйтесь
            </div>
        </div>
    </div>
    <div v-if="user_type == 'User'" id="login_user">
        <input class="form-group" placeholder="id комнаты">
        <input class="form-group" placeholder="Ваше имя">
        <div :style="'visibility:' + check_failed()" id="failed_message_user">Неверный id комнаты</div>
        <div id="enter">
            Войти
        </div>
        <div id="user_message">
                Для входа запросите id комнаты у своего ведущего
        </div>
    </div> -->
  </div>
</div>
</template>

<script>
import {SERVER_PATH} from "@/common_const";

export default {
  name: 'AuthWin',
  data(){
    return {
        user_type: 'Master',
        failed: false
    }
  },
  methods: {
        select_tab_1: function () {
            if(this.user_type != 'Master')
                this.failed = false;
            document.getElementById('tab_1').style.textDecoration = 'underline';
            document.getElementById('tab_2').style.textDecoration = 'none';
            this.user_type = 'Master'
        },
        select_tab_2: function () {
            if(this.user_type != 'User')
                this.failed = false;
            document.getElementById('tab_2').style.textDecoration = 'underline';
            document.getElementById('tab_1').style.textDecoration = 'none';
            this.user_type = 'User'
            document.getElementById('failed_message_master').style.visibility = 'hidden';
        },
        log_in: function () {
          let email = document.getElementById('email_master').value;
          let password = document.getElementById('password').value;
          let formData = new FormData()
          formData.append("username", email)
          formData.append("password", password)
            if(email == 'ведущий' && password == 'ведущий123')
                this.$emit('login-master');
            else if(email == 'admin' && password == 'admin')
              fetch(`${SERVER_PATH}/auth/login`, {
                method: 'POST',
                body: formData
              }).then( res => {res.json(); this.$emit('login-admin')} ).then( data => console.log(data) );
        },
        check_failed: function () {
            if(this.failed)
                return 'visible';
            else
                return 'hidden';
        },
  },
  mounted: function () {
  this.$nextTick(function () {
    // Код, который будет запущен только после
    // отображения всех представлений
    document.getElementById('tab_1').style.textDecoration = 'underline';
    this.user_type = 'Master'
  })
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.Auth{
    height: 100%;
    width: 100%;
}

.AuthHeader{
    color: #f9fff9;
    font-size: 250%;
    margin-top: 3%;
    text-align: center;
}

.AuthLogo{
    color: #3db33d;
    font-size: 550%;
    margin-top: 2.5%;
    text-align: center;
}

.group_auth{
    width: 33%;
    height: 65%;
    background: #21A038;
    border-radius: 15px;
    margin-top: 3%;
    border: solid 2px;
    margin-left: 33%;
}

#login_master{
    width: 100%;
    height: 40%;
}

#login_user{
    width: 100%;
    height: 40%;
}

#group_tabs{
    width: 100%;
    height: 8%;
    margin-top: 2%;
}

.tab{
    width: 45%;
    height: 100%;
    float: left;
    text-align: center;
    color: #f9fff9;
    font-size: 150%;
    margin-left: -1%;
}

#tab_1{
    margin-left: 1%;
}

#tab_2{
    width: 55%;
}

.form-group  {
  width: 80%;
  height: 20%;
  border-radius: 25px;
  margin-left: 10%;
  margin-top: 5%;
  font-size: 120%;
  border: solid 2px;
  text-indent: 5%;
}

#enter{
    margin-left: 25%;
    width: 50%;
    height: 25%;
    border: solid black 2px;
    background: #007A10;
    color: #f9fff9;
    font-size: 175%;
    text-align: center;
    border-radius: 15px;
    line-height: 150%;
    margin-top: 4%;
}

#get_pass{
    text-align: center;
    margin-top: 4%;
    font-size: 120%;
    color: #f9fff9;
}

#registration{
    margin-left: 10%;
    width: 80%;
    text-align: center;
    height: 5%;
    margin-top: 2%;
    font-size: 120%;
}

#reg_text{
    width: 55%;
    float: left;
}

#go_to_reg{
    width: 45%;
    float: left;
    color: #f9fff9;
}

#user_message{
    text-align: center;
    margin-top: 4%;
    font-size: 120%;
    color: #f9fff9;
}

#failed_message_master, #failed_message_user{
    color: #f9fff9;
    text-align: center;
    margin-top: 1%;
}
</style>