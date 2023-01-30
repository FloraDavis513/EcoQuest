<template>
<div class="container-login100">
    <div class="wrap-login100 col-sm-3 mx-auto">             
            <div class="login100-form-title">
                <div class="green_part">Э</div><div class="black_part">ко</div><div class="green_part">К</div><div class="black_part">вест</div>
            </div>

            <div class="login100-">
                <div class="form-group">
                    <label class="form-label" for="name">Фамилия</label>
                    <input type="text" class="form-control" id="lastname" placeholder="Введите фамилию">
                </div>
                <div class="form-group">
                    <label class="form-label" for="name">Имя</label>
                    <input type="text" class="form-control" id="firstname" placeholder="Введите имя">
                </div>
                <div class="form-group">
                    <label class="form-label" for="name">Отчество</label>
                    <input type="text" class="form-control" id="middlename" placeholder="Введите отчество">
                </div>

                <div class="form-group">
                    <label class="form-label" for="login">Логин</label>
                    <input type="text" class="form-control" id="login" placeholder="Введите логин">
                </div>

                <div class="form-group">
                    <label class="form-label" for="password">Пароль</label>
                    <input type="password" class="form-control" id="password" placeholder="Введите пароль" @change="change_pass">
                </div>

                <div class="form-group">
                    <label class="form-label" for="repeatPassword">Повторите пароль</label>
                    <input type="password" class="form-control" id="repeatPassword" placeholder="Введите пароль" @change="change_pass">
                </div>

                <div v-show="non_equal" id="warning">{{warning_message}}</div>

                <div class="login_button" @click="go_reg">Зарегистрироваться</div>
                <div @click="already_registered" class="alreadyLogin_button">У меня уже есть аккаунт</div>
            </div>                             
    </div>      
</div>
</template>

<script>
import { SERVER_PATH } from './common_const.js'

export default {
  name: 'RegistrationForm',
  data(){
    return {
        non_equal: false,
        warning_message: 'Пароли не совпадают'
    }
  },
  methods:
  {
    already_registered: function()
    {
        this.$emit('already-registered');
    },
    go_reg: function()
    {
        let username = document.getElementById("login").value;
        let firstname = document.getElementById("firstname").value;
        let middlename = document.getElementById("middlename").value;
        let lastname = document.getElementById("lastname").value;
        let password = document.getElementById("password").value;
        let rep_password = document.getElementById("repeatPassword").value;
        this.change_pass();
        if(!this.is_correct_data(username, firstname, middlename, lastname, password, rep_password) || this.non_equal)
        {
            return;
        }
        fetch(SERVER_PATH + "/user/create", {
            method: "POST",
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(
                {login: username, 
                password: password, 
                firstName: firstname,
                patronymic: middlename,
                lastName: lastname})
            });
        alert("Заявка на регистрацию отправлена администатору.");
        this.$emit('already-registered');
    },
    is_correct_data: function(username, firstname, middlename, lastname, password, rep_password)
    {
        let mistakes = 0;
        if(!username || username == '')
        {
            document.getElementById("login").style.borderBottom = "0.15vw solid red";
            this.warning_message = 'Некорректный логин'
            ++mistakes;
        }
        else
            document.getElementById("login").style.borderBottom = "0.1vw solid silver";
        if(!firstname || firstname == '')
        {
            document.getElementById("firstname").style.borderBottom = "0.15vw solid red";
            this.warning_message = 'Некорректное имя'
            ++mistakes;
        }
        else
            document.getElementById("firstname").style.borderBottom = "0.1vw solid silver";
        if(!middlename || middlename == '')
        {
            document.getElementById("middlename").style.borderBottom = "0.15vw solid red";
            this.warning_message = 'Некорректное отчество'
            ++mistakes;
        }
        else
            document.getElementById("middlename").style.borderBottom = "0.1vw solid silver";
        if(!lastname || lastname == '')
        {
            document.getElementById("lastname").style.borderBottom = "0.15vw solid red";
            this.warning_message = 'Некорректная фамилия'
            ++mistakes;
        }
        else
            document.getElementById("lastname").style.borderBottom = "0.1vw solid silver";
        if(!password || password == '')
        {
            document.getElementById("password").style.borderBottom = "0.15vw solid red";
            this.warning_message = 'Некорректный пароль'
            ++mistakes;
        }
        else if(!this.non_equal)
            document.getElementById("password").style.borderBottom = "0.1vw solid silver";
        if(!rep_password || rep_password == '')
        {
            document.getElementById("repeatPassword").style.borderBottom = "0.15vw solid red";
            this.warning_message = 'Некорректный пароль'
            ++mistakes;
        }
        else if(!this.non_equal)
            document.getElementById("repeatPassword").style.borderBottom = "0.1vw solid silver";
        if(mistakes > 0)
        {
            if(mistakes > 1)
                this.warning_message = 'Введены некорректные данные';
            this.non_equal = true;
            return false;
        }
            
        return true;
    },
    change_pass: function()
    {
        console.log("updated");
        let pass = document.getElementById("password");
        let rep_pass = document.getElementById("repeatPassword");
        if(pass && rep_pass)
        {
            let pass_val = pass.value;
            let rep_pass_val = rep_pass.value;
            if( pass_val != '' && rep_pass_val != '' && pass_val != rep_pass_val )
            {
                this.warning_message = 'Пароли не совпадают';
                this.non_equal = true;
                pass.style.borderBottom = "0.15vw solid red";
                rep_pass.style.borderBottom = "0.15vw solid red";
            }
            else
            {
                this.non_equal = false;
                pass.style.borderBottom = "0.1vw solid silver";
                rep_pass.style.borderBottom = "0.1vw solid silver";
            }
        }
    }
  }
}
</script>

<style scoped>
.login100-form {
    width: 100%;
}

.login100-form-title {
    display: block;
    font-size: 4.5vw;
    color: #333333;
    text-align: center;
    margin-bottom: 8%;
    display: flex;
    align-items: center;
    justify-content: center;
}

.green_part{
    float: left;
    color: rgb(33, 160, 56);
}

.black_part{
    float: left;
    color: #000000;
}

.login_button {
    margin-top: 5%;
    margin-left: 25%;
    width: 50%;
    text-align: center;
    display: block;
    background: rgb(33, 160, 56);
    border: 0;
    color: white;
    border-radius: 1vw;
    outline: 1px solid groove;
    padding-top: 3.2%;
    padding-bottom: 3.2%;
    padding-left: 0.2%;
    padding-right: 0.2%;
    font-size: 1vw;
    font-weight: bold;
}

.login_button:hover {
    box-shadow: 0 0 10px 100px orange inset;
}

.alreadyLogin_button {
    margin-top: 2%;
    margin-left: 25%;
    width: 50%;
    text-align: center;
    display: block;
    background: white;
    border: 0.1vw solid rgb(33, 160, 56);
    color: rgb(33, 160, 56);
    border-radius: 1vw;
    padding-top: 3%;
    padding-bottom: 3%;
    font-size: 1vw;
    font-weight: bold;
}

.alreadyLogin_button:hover {
    box-shadow: 0 0 10px 100px rgba(255, 165, 0, 0.5) inset;
}

.wrap-login100 {
    width: 35%;
    height: 95%;
    background: #fff;
    border-radius: 0.75vw;
    border: solid 0.2vw black;
    overflow: hidden;
}
.container-login100 {
    width: 100%;
    height: 100%;
    display: -webkit-box;
    display: -webkit-flex;
    display: -moz-box;
    display: -ms-flexbox;
    display: flex;
    flex-wrap: wrap;
    justify-content: center;
    align-items: center;
}
.form-control{
    width: 90%;
    margin-left: 5%;
    font-size: 1.3vw;
    border-bottom: 0.1vw solid silver;
    border-top: none;
    border-left: none;
    border-right: none;
    outline:none;
    margin-top: 0.5%;
}
.form-label{
    margin-left: 5%;
    font-size: 1.3vw;
}
.form-group{
    margin-top: 3%;
}

#warning{
    text-align: center;
    color: red;
    font-size: 0.85vw;
}
</style>