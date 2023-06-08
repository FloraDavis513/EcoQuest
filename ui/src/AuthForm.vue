<template>
<div class="Auth">
  <div class="AuthLogo">
      <div class="green_part">Э</div><div class="black_part">ко</div><div class="green_part">К</div><div class="black_part">вест</div>
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
    <div v-if="user_type == 'Master'" id="login_master">
        <input class="form-group" id="email_master" placeholder="Логин" v-on:keyup.enter="log_in">
        <input type="password" class="form-group" id="password" placeholder="Пароль" v-on:keyup.enter="log_in">
        <div :style="'visibility:' + check_failed()" id="failed_message_master">Неверный логин или пароль</div>
        <div id="enter" @click="log_in">
            Войти
        </div>
        <div id="get_pass">
                Забыли пароль?
        </div>
        <div id="registration">
            <div id="reg_text">
            Ещё нет аккаунта?
            </div>
            <div id="go_to_reg" @click="go_to_reg">
            Зарегистрируйтесь
            </div>
        </div>
    </div>
    <div v-if="user_type == 'User'" id="login_user">
        <input class="form-group" id="room_id" placeholder="id комнаты">
        <input class="form-group" id="player_name" placeholder="Ваше имя">
        <div :style="'visibility:' + check_failed()" id="failed_message_user">Неверный id комнаты</div>
        <div id="enter" @click="log_in_player">
            Войти
        </div>
        <div id="user_message">
                Для входа запросите id комнаты у своего ведущего
        </div>
    </div>
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
        failed: false,
        game: null
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
        log_in: async function () {
            var email = document.getElementById('email_master').value;
            var password = document.getElementById('password').value;
            await fetch(SERVER_PATH + '/authentication/login/master', {
                method: 'POST',
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({login:email, password:password}),
              })
              .then(response => {
                    if (response.status == 404 || response.status == 401) {
                        this.failed = true;
                        return {};
                    }
                    this.failed = false;
                    return response.json();
                })
              .then( data => localStorage.setItem('user', JSON.stringify(data)));
            if(JSON.parse(localStorage.getItem('user')).role == 'master')
                this.$emit('login-master');
            if(JSON.parse(localStorage.getItem('user')).role == 'admin')
                this.$emit('login-admin');
            if(JSON.parse(localStorage.getItem('user')).role == 'player')
                this.$emit('login-player');
        },
        log_in_player: async function () {
            var room_id = document.getElementById('room_id').value;
            var player_name = document.getElementById('player_name').value;
            await fetch(SERVER_PATH + '/authentication/login/player', {
                method: 'POST',
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({gameId:room_id, login:player_name}),
              })
              .then(response => {
                    if (response.status == 404 || response.status == 401) {
                        this.failed = true;
                        return {};
                    }
                    return response.json();
                })
              .then( data => localStorage.setItem('user', JSON.stringify(data)));

            //   await fetch(SERVER_PATH + "/game/get/" + String(room_id), {
            //     method: "GET",
            //     headers: {'Content-Type': 'application/json'}
            //     }).then( res => res.json() ).then( data => this.game = data );
            // this.game.state = JSON.parse(this.game.state);
            // this.game.state.joined_players.push({name: player_name, id: player_name + String(Date.now())});

            await fetch(SERVER_PATH + '/game/state/players/create/' + room_id, {
                method: 'POST',
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({Login:player_name, List: 0}),
              });
              localStorage.setItem('room_id', room_id)

              this.$emit('create-game', room_id);
        },
        check_failed: function () {
            if(this.failed)
                return 'visible';
            else
                return 'hidden';
        },
        go_to_reg: function () {
            this.$emit('go-to-reg');
        },
  },
  mounted: function () {
    localStorage.clear();
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
    font-size: 2.5vmax;
    margin-top: 3%;
    text-align: center;
}

.green_part{
    float: left;
    color: rgb(33, 160, 56);
}
.black_part{
    float: left;
    color: #000000;
}

.AuthLogo{
    font-size: 5.5vmax;
    margin-top: 2.5%;
    text-align: center;
    width: 100%;
    height: 10%;
    display: flex;
  align-items: center;
  justify-content: center;
}

.group_auth{
    width: 33.3%;
    height: 65%;
    background: #21A038;
    border-radius: 1vw;
    margin-top: 3%;
    border: solid 0.15vw;
    margin-left: 33.3%;
}

@media screen and (max-width: 800px) {
  .group_auth {
    width: 90%;
    height: 50%;
    margin-left: 5%;
  }
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
    font-size: 1.75vmax;
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
    border-radius: 1.5vw;
    margin-left: 10%;
    margin-top: 5%;
    font-size: 1.3vmax;
    border: solid 0.15vw;
    text-indent: 5%;
}

#enter{
    padding-top: 2.5%;
    padding-bottom: 2.5%;
    margin-left: 25%;
    width: 50%;
    border: solid black 2px;
    background: #007A10;
    color: #f9fff9;
    font-size: 1.75vmax;
    text-align: center;
    border-radius: 15px;
    margin-top: 4%;
}
#enter:hover {
    box-shadow: 0 0 10px 100px orange inset;
}

#get_pass{
    text-align: center;
    margin-top: 4%;
    font-size: 1.2vmax;
    color: #f9fff9;
}

#registration{
    margin-left: 10%;
    width: 80%;
    text-align: center;
    height: 5%;
    margin-top: 2%;
    font-size: 1.2vmax;
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
    font-size: 1.2vmax;
    color: #f9fff9;
}

#failed_message_master, #failed_message_user{
    color: #f9fff9;
    text-align: center;
    margin-top: 1%;
}

#go_to_reg:hover, #get_pass:hover {
    text-decoration: underline;
}
</style>