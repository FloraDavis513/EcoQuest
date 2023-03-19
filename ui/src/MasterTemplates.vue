<template>
    <MasterHeader @logout="log_out" @change-pass="change_pass = true" />
    <MasterMenu v-if="draw == 'settings'" @select-question= "visible1" @select-themes= "visible2" @back-to-templates="back_to_templates" @save-tmpl-name="save_tmpl_name" @check-delete="check_delete" :template="current_template" :count="current_template_count" :questions="current_questions" @share-template="share_template" />
    <TemplateList v-if="draw == 'grid'" :template_list="user_templates" @select-template="select_template" @add-template="add_template" />
    <TemplateMenu v-if="draw == 'settings'" @round-1="round_1" @round-2="round_2" @games="games" />
    <TemplateSettings v-if="draw == 'settings'" ref="settings" :visible="visible" :current_round="current_round" :template="current_template" @create-game="create_game" @change-count="set_count" @to-questions="visible1" @back-to-templates="back_to_templates" @show-game="show_game"  @games="games" />
    <div id="question_preview" v-show="show_masters">
        <div style="float:right;font-size:1.75vw;margin-top:0.25%;margin-right:2.5%;width:10%;text-align:right;" @click="close_share">x</div>
        <div v-show="success_share" id="share_with" style="float:left;font-size:1.25vw;margin-top:0.25%;margin-left:5%;width:82.5%;font-style:italic;"/>
        <div class="scroll">
            <div class="master_row" v-for="(option, index) in masters" :key="index" @click="select_master(option)">{{option.lastName + " " + option.firstName + " " + option.patronymic}}</div>
        </div>
    </div>
    <div id="question_preview" v-show="change_pass">
        <div style="float:right;font-size:1.75vw;margin-top:0.25%;margin-right:2.5%;width:10%;text-align:right;" @click="close_changes_pass">x</div>
        <div style="width:90%;margin-left:5%;font-size:1.5vw;margin-top:2.5%;">Введите новый пароль:</div>
        <input type="password" id="new_pass" style="width:90%;margin-left:5%;font-size:1.5vw;margin-top:1%;border-bottom:0.1vw solid silver;border-top:none;border-left:none;border-right:none;outline:none;" placeholder="Новый пароль">
        <div style="width:90%;margin-left:5%;font-size:1.5vw;margin-top:2.5%;">Для подтверждения действия введите старый пароль:</div>
        <input type="password" id="old_pass" style="width:90%;margin-left:5%;font-size:1.5vw;margin-top:1%;border-bottom:0.1vw solid silver;border-top:none;border-left:none;border-right:none;outline:none;" placeholder="Старый пароль">
        <div v-show="error_change" id="change_status" style="float:left;color:red;margin-top: 2%;margin-left: 5%;font-size:1.1vw;"></div>
        <div class="button" style="float:right;" @click="update_pass">Сохранить</div>
    </div>
</template>

<script>
import MasterHeader from './components/MasterHeader.vue'
import MasterMenu from './components/MasterMenu.vue'
import TemplateList from './components/TemplateList.vue'
import TemplateSettings from './components/TemplateSettings.vue'
import TemplateMenu from './components/TemplateMenu.vue'
import { SERVER_PATH } from './common_const.js'

export default {
  name: 'MasterTemplates',
  components: {
    MasterHeader,
    TemplateList,
    TemplateSettings,
    MasterMenu,
    TemplateMenu
  }, 
  data(){
    return {
      visible: 1,
      current_round: 1,
      draw: 'grid',
      current_template: {name:""},
      current_template_count: null,
      current_questions: null,
      products: [],
      user_templates: [],
      show_masters: false,
      masters: [],
      success_share: false,
      change_pass: false,
      error_change: false
    }
  },
  methods:{
    update_pass: function () {
        fetch(SERVER_PATH + "/user/update/password", {
            method: "POST",
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify({Login: JSON.parse(localStorage.getItem('user')).login, NewPassword: document.getElementById('new_pass').value, OldPassword: document.getElementById('old_pass').value})
            }).then(res => {
              if(res.status !== 200)
              {
                  this.error_change = true;
                  res.text().then(text => this.set_error_message(text));
              }
              else
              {
                  this.error_change = true;
                  res.text().then(this.set_error_message("Пароль успешно обновлён"));
              }
          });
    },
    set_error_message: function (message) {
        document.getElementById("change_status").innerHTML = message;
    },
    close_changes_pass: function () {
        this.change_pass = false;
        document.getElementById('new_pass').value = '';
        document.getElementById('old_pass').value = '';
    },
    close_share: function(){
        this.show_masters = false;
        this.success_share = false;
    },
  select_master: function(whom){
      fetch(SERVER_PATH + "/gameBoard/share/" + JSON.parse(localStorage.getItem('user')).userId + "/" + this.current_template.gameBoardId + "/" + whom.userId, {
            method: "POST",
            headers: {'Content-Type': 'application/json'},
            body: "{}"
            });
        this.success_share = true;
        document.getElementById("share_with").innerText = 'Вы успешно поделились шаблоном с ' + whom.lastName + " " + whom.firstName[0] + ". " + whom.patronymic[0] + '.';
    },
    share_template: function(){
      fetch(SERVER_PATH + "/user/get/activeMasters", {
            method: "GET",
            headers: {'Content-Type': 'application/json'}
            }).then( res => res.json() ).then( data => this.masters = data);
        this.show_masters = true;
    },
    round_1: function(){
        this.visible = 1;
        this.current_round = 1;
    },
    round_2: function(){
        this.visible = 1;
        this.current_round = 2;
    },
    games: function(){
        this.visible = 4;
        this.$refs.settings.read_games();
    },
    show_game: function(){
        this.visible = 5;
    },
    log_out: function(){
        this.$emit('logout');
    },
    create_game: function(game_id){
        this.$emit('create-game', game_id);
    },
    visible1: function () { // переключение на вкладку выбор вопросов
      this.visible = 1;
      this.$refs.settings.count_field_now();
    },
    visible2: function () { // переключение на вкладку выбор тем
      this.visible = 2
      this.$refs.settings.count_field_now();
    },
    check_delete: function () { // переключение на вкладку выбор тем
      this.visible = 3
      this.$refs.settings.count_field_now();
    },
    select_template: async function (tmpl) {
      await fetch(SERVER_PATH + "/gameBoard/get/" + String(tmpl.gameBoardId), {
            method: "GET",
            headers: {'Content-Type': 'application/json'}
            }).then( res => res.json() ).then( data => this.current_template = data );
      this.draw = 'settings';
    },
    back_to_templates: function () {
      this.read_user_templates();
      this.draw = 'grid';
    },
    add_template: async function () {
      let current_products;
      let second_round_filled = false;
      await fetch(SERVER_PATH + "/product/get/all", {
            method: "GET",
            headers: {'Content-Type': 'application/json'}
            }).then( res => res.json() ).then( data => current_products = data );
      current_products.forEach(product => {
          let selected_question = [];
          product.questions.forEach(question => {
              selected_question.push(question.questionId);
          });
          product.activeQuestions = selected_question;
          product.allQuestions = product.questions;
          delete product.questions;
          if(!second_round_filled && product.round == 2)
          {
            product.numOfRepeating = 6;
            second_round_filled = true;
          }
      });
      let new_template = {userId: JSON.parse(localStorage.getItem('user')).userId, name:'Новый шаблон', numFields: 16, products:current_products};
      await fetch(SERVER_PATH + "/gameBoard/create", {
        method: "POST",
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(new_template)
      });
      this.read_user_templates();
    },
    set_count: function (data) {
      this.current_template.numFields = data;
    },
    save_tmpl_name: function (data) {
      this.current_template.name = data;
    },
    read_user_templates: function () {
      fetch(SERVER_PATH + "/gameBoard/get/all/" + JSON.parse(localStorage.getItem('user')).userId, {
            method: "GET",
            headers: {'Content-Type': 'application/json'}
            }).then( res => res.json() ).then( data => this.user_templates = data );
    },
  }, 
  beforeMount: async function () {
      if(!localStorage.getItem('user'))
          this.log_out();
      await fetch(SERVER_PATH + "/templates", {
                method: "GET",
                headers: {'Content-Type': 'application/json', "Authorization": "Bearer " + JSON.parse(localStorage.getItem('user')).authorizationToken}
        }).then(res => {
            if(res.status === 401 || res.status === 403)
                this.log_out();
        });
      
      this.read_user_templates();
  },
  beforeCreate: async function () {
        if(!localStorage.getItem('user'))
          this.log_out();
        await fetch(SERVER_PATH + "/templates", {
                method: "GET",
                headers: {'Content-Type': 'application/json', "Authorization": "Bearer " + JSON.parse(localStorage.getItem('user')).authorizationToken}
        }).then(res => {
            if(res.status === 401 || res.status === 403)
                this.log_out();
        });
    }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
#question_preview{
  position: absolute;
  background-color: white;
  border: 0.2vw solid black;
  left: 27.5%;
  width: 45%;
  top: 30%;
  height: 40%;
  border-radius: 1vw;
}

.scroll{
    height: 85%;
    overflow: auto;
    -ms-overflow-style: none;
    scrollbar-width: none;
    margin-top: 1%;
    width: 100%;
}

.scroll::-webkit-scrollbar {
    width: 0;
    height: 0;
}

.master_row{
  font-size: 2vw;
  margin-left: 5%;
  margin-top: 1.5%;
}

.master_row:hover{
  text-decoration: underline;
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
</style>