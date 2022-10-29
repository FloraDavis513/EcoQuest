<template>
    <div class="main_field" id="requests">
        <div class="master_data" v-if="master_chosen">
            <div class="go_back" @click="go_back">
                <img src="@/assets/go_back.png" alt="">
            </div>
            <div v-if="state == 'edit'">
                <div class="group_inputs">
                    <div class="inputs">
                        <input id="header_fio" value="ФИО"/><input id="fio" :value="fio"/>
                    </div>
                    <div class="inputs">
                        <input id="header_fio" value="Логин"/><input id="login" :value="login"/>
                    </div>
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
            <div v-if="state == 'confirm_delete'">
                <div id="delete_header">Вы уверены, что хотите удалить ведущего {{fio}}?</div>
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
                
                <div class="option_requests" v-for="(option, index) in options" :key="index" v-bind:value="option.value" v-bind:name="option.text">
                    <div class="master_name">{{ option.text }}</div>
                    <center><div class="accept" @click="add"><img src="@/assets/accept.png" alt=""></div></center>
                    <center><div class="reject" @click="remove"><img src="@/assets/reject.png" alt=""></div></center>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
export default {
  name: 'MasterRequests',
  props: ['master_chosen', 'selected', 'fio', 'login'],
  data(){
    return {
        options: [
        { text: 'Ведущий на добавление 1', value: '13' },
        { text: 'Ведущий на добавление 2', value: '14' },
        { text: 'Ведущий на добавление 3', value: '15' },
        { text: 'Ведущий на добавление 4', value: '16' },
        { text: 'Ведущий на добавление 5', value: '17' },
        { text: 'Ведущий на добавление 6', value: '18' },
        { text: 'Ведущий на добавление 7', value: '19' },
        { text: 'Ведущий на добавление 8', value: '20' },
        { text: 'Ведущий на добавление 9', value: '21' },
        { text: 'Ведущий на добавление 10', value: '22' },
        { text: 'Ведущий на добавление 11', value: '23' },
        { text: 'Ведущий на добавление 12', value: '24' },
        ],
        counter: 0,
        draw: 'edit',
        state: 'edit'
    }
  },
  methods: {
        add: function (event) {
            var added_element = event.target.parentElement.parentElement.parentElement;
            var val = added_element.getAttribute("value");
            var name = added_element.getAttribute("name");
            this.$emit('add-master', name, val);
            this.options = this.options.filter(option => option.value != val);
        },
        remove: function (event) {
            var deleted_element = event.target.parentElement.parentElement.parentElement.getAttribute("value");
            this.options = this.options.filter(option => option.value != deleted_element);
        },
        check_remove_old_master: function () {
            this.state = 'confirm_delete'
        },
        remove_old_master: function () {
            var value = this.selected.getAttribute("value");
            this.$emit('remove-master', value);
            this.state = 'edit'
        },
        cancel_delete: function () {
            this.state = 'edit'
        },
        edit: function () {
            var fio = document.getElementById("fio");
            var login = document.getElementById("login");
            fio.style.pointerEvents = 'auto';
            login.style.pointerEvents = 'auto';
            this.draw = 'save_edit';
        },
        go_back: function () {
            this.$emit('go-back');
            this.state = 'edit'
        },
        save_edit: function () {
            this.draw = 'edit';
            var fio = document.getElementById("fio");
            var login = document.getElementById("login");
            fio.style.pointerEvents = 'none';
            login.style.pointerEvents = 'none';
            this.$emit('save-edit', fio.value);
        },
    },
    created: function () {
        this.counter = this.options.length;
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
    margin-top: 3%;
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

.inputs{
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
}

#header_fio{
    width: 8%;
    border-bottom: 1.5px solid silver;
    border-top: none;
    border-left: none;
    border-right: none;
    outline:none;
    pointer-events: none;
    font-size: 1.3vw;
}

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
    margin-left: 12%;
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
</style>
