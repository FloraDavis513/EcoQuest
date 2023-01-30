<template>
    <div class="main_field" id="requests">
        <div class="master_data" v-if="master_chosen">
            <div class="go_back" @click="go_back">
                <img src="@/assets/go_back.png" alt="">
            </div>
            <div v-if="state == 'edit'">
                <div class="group_inputs">
                    <div class="form-group">
                        <label class="form-label" for="name">Фамилия</label>
                        <input type="text" class="form-control" id="lastname" placeholder="Введите фамилию" :value="selected.lastName" >
                    </div>
                    <div class="form-group">
                        <label class="form-label" for="name">Имя</label>
                        <input type="text" class="form-control" id="firstname" placeholder="Введите имя" :value="selected.firstName">
                    </div>
                    <div class="form-group">
                        <label class="form-label" for="name">Отчество</label>
                        <input type="text" class="form-control" id="middlename" placeholder="Введите отчество" :value="selected.patronymic">
                    </div>

                    <div class="form-group">
                        <label class="form-label" for="login">Логин</label>
                        <input type="text" class="form-control" id="login" placeholder="Введите логин" :value="selected.login">
                    </div>

                    <!-- <div class="form-group">
                        <label class="form-label" for="password">Пароль</label>
                        <input type="password" class="form-control" id="password" placeholder="Введите пароль" @change="change_pass">
                    </div> -->
                </div>
                <div class="group_button">
                    <div v-if="draw == 'edit'" @click="edit" class="button">
                        Редактировать
                    </div>
                    <div v-if="draw == 'save_edit'" @click="save_edit" class="button">
                        Сохранить
                    </div>
                    <div @click="check_remove_old_master" class="button">
                        Удалить
                    </div>
                </div>
            </div>
            <div v-if="state == 'confirm_delete'" id="delete_group">
                <div id="delete_header">Вы уверены, что хотите удалить ведущего {{selected.firstName + ' ' + selected.patronymic + ' ' + selected.lastName}}?</div>
                <div class="group_button">
                    <div @click="remove_old_master" class="button">
                        Да
                    </div>
                    <div @click="cancel_delete" class="button">
                        Нет
                    </div>
                </div>
            </div>
        </div>
        <div v-else class="new_requests">
            <div class="requests_title">Заявки на добавление ведущих</div>
            <div class="scroll_1">
                
                <div class="option_requests" v-for="(option, index) in options" :key="index" v-bind:value="option.userId" v-bind:name="option.userId">
                    <div class="master_name">{{ option.firstName + ' ' + option.patronymic + ' ' + option.lastName }}</div>
                    <center><div class="accept" @click="add(index)"><img src="@/assets/accept.png" alt=""></div></center>
                    <center><div class="reject" @click="remove"><img src="@/assets/reject.png" alt=""></div></center>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import { SERVER_PATH } from '../common_const.js'

export default {
  name: 'MasterRequests',
  props: ['master_chosen', 'selected'],
  data(){
    return {
        options: [ ],
        counter: 0,
        draw: 'edit',
        state: 'edit'
    }
  },
  methods: {
        add: function (add_index) {
            console.log(add_index);
            var change_user = this.options[add_index];
            this.$emit('add-master', change_user);
            this.options = this.options.filter((option, index) => index != add_index);
            fetch(SERVER_PATH + "/user/toActiveMaster/" + String(change_user.userId), {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                })
        },
        remove: function (event) {
            var deleted_id = event.target.parentElement.parentElement.parentElement.getAttribute("value");
            this.options = this.options.filter(option => option.userId != deleted_id);
            fetch(SERVER_PATH + "/user/delete/" + String(deleted_id), {
                method: "DELETE",
                headers: {'Content-Type': 'application/json'}
                });
        },
        check_remove_old_master: function () {
            this.state = 'confirm_delete'
        },
        remove_old_master: function () {
            this.$emit('remove-master');
            this.state = 'edit'
        },
        cancel_delete: function () {
            this.state = 'edit'
        },
        edit: function () {
            this.draw = 'save_edit';
            var list = document.getElementsByClassName("form-control");
            for (let item of list) {
                item.style.pointerEvents = 'auto';
            }
        },
        go_back: function () {
            this.$emit('go-back');
            this.state = 'edit'
        },
        save_edit: function () {
            this.draw = 'edit';
            var firstname = document.getElementById("firstname").value;
            var middlename = document.getElementById("middlename").value;
            var lastname = document.getElementById("lastname").value;
            var login = document.getElementById("login").value;
            var list = document.getElementsByClassName("form-control");
            for (let item of list) {
                item.style.pointerEvents = 'none';
            }
            this.$emit('save-edit', firstname, middlename, lastname, login);
        },
    },
    created: function () {
        this.counter = this.options.length;
    },
    beforeMount: function () {
        let masters_ref = this.options;
        masters_ref.length = 0;
        this.$nextTick(function () {

        fetch(SERVER_PATH + "/user/get/inactiveMasters", {
                method: "GET",
                headers: {'Content-Type': 'application/json'}
                }).then( res => res.json() ).then( data => data.forEach(function(item) {
                   masters_ref.push(item)}) );
        });
    },
    updated: function() {
        if(this.draw == 'save_edit')
            return;
        var list = document.getElementsByClassName("form-control");
        for (let item of list) {
            item.style.pointerEvents = 'none';
        }
    },
    watch: {
        selected: function () {
            this.state = 'edit';
            this.draw = 'edit';
        }
    }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.main_field{
    width: 77%;
    height: 89%;
    margin-left: 20%;
}

.master_data{
    width: 100%;
    height: 90%;
    margin-top: 4.5%;
}

.new_requests{
    float: left;
    width: 90%;
    text-align: center;
    height: 95%;
    padding-top: 1%;
    padding-bottom: 1%;
    margin-left: 1%;
}
.requests_title{
    font-size: 1.5vw;
    font-weight: bold;
    height: 5%;
    margin-left: 12%;
}

.scroll_1{
    height: 95%;
    overflow: auto;
    -ms-overflow-style: none;
    scrollbar-width: none;
    width: 100%;
    margin-left: 6%;
}

.scroll_1::-webkit-scrollbar {
    width: 0;
    height: 0;
}

.option_requests{
    height: 5%;
    text-indent: 5%;
    padding-top: 0.7%;
    padding-bottom: 0.7%;
    text-align: left;
    border-radius: 35px;
    background-color: silver;
    color: #000000;
    font-size: 1.3vw;
    margin: 1.5%;
    font-weight: bold;
    display: table;
    width: 95%;
}

/* Manage masters */

.master_name{
    float: left;
    width: 84%;
    height: 100%;
    margin-top: 0.75%;
}

.accept{
    float: left;
    width: 7%;
    height: 100%;
    margin-top: 0.5%;
}

.reject{
    float: left;
    width: 7%;
    margin-right: 2%;
    margin-top: 0.5%;
    height: 100%;
}

img{
    width: 45%;
    height: 45%;
    transition: transform .25s ease;
}

img:hover{
    transform: scale(1.2); /* (150% zoom - Note: if the zoom is too large, it will go outside of the viewport) */
}

/* chosen master */

.go_back{
    float: left;
    height: 8%;
    width: 8%;
}

.go_back img{
    height: 70%;
    width: 70%;
}

/* .inputs{
    float: left;
    width: 83.5%;
    margin-top: 2%;
    margin-bottom: 2%;
    margin-left: 5%;
}

.inputs input{
    width: 80%;
    border-bottom: 1.5px solid silver;
    border-top: none;
    border-left: none;
    border-right: none;
    outline:none;
    pointer-events: none;
    font-size: 1.3vw;
} */

/* #header_fio{
    width: 8%;
    border-bottom: 1.5px solid silver;
    border-top: none;
    border-left: none;
    border-right: none;
    outline:none;
    pointer-events: none;
    font-size: 1.3vw;
} */

.prefix input{
    width: 2%;
}

/* manage data masters */

.group_inputs{
    float: left;
    width: 90%;
    height: 25%;
}

.group_button{
    float: left;
    width: 80%;
    margin-left: 17.5%;
    margin-bottom: 3%;
}

.button{
    width: 35%;
    float: left;
    margin-top: 2%;
    margin-right: 4%;
    margin-left: 4%;
    background-color: green;
    color: #ffffff;
    font-size: 1.2vw;
    font-weight: bold;
    border-radius: 35px;
    padding-top: 1%;
    padding-bottom: 1%;
    text-align: center;
}

.button:hover {
    box-shadow: 0 0 10px 100px orange inset;
}

.table{
    margin-left: 12%;
    width: 85%;
}

caption{
    margin-bottom: 1%;
    font-size: 1.2vw;
    font-weight: bold;
}

#delete_header{
    text-align: center;
    font-size: 1.5vw;
    width: 90%;
}

#delete_group{
    margin-top: 4.5%;
    float: left;
    width: 90%;
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
</style>
