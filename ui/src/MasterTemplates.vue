<template>
    <MasterHeader @logout="log_out" />
    <MasterMenu v-if="draw == 'settings'" @select-question= "visible1" @select-themes= "visible2" @back-to-templates="back_to_templates" @save-tmpl-name="save_tmpl_name" @check-delete="check_delete" :template="current_template" :count="current_template_count" :questions="current_questions" />
    <TemplateList v-if="draw == 'grid'" :template_list="user_templates" @select-template="select_template" @add-template="add_template" />
    <TemplateMenu v-if="draw == 'settings'" @round-1="round_1" @round-2="round_2" @games="games" />
    <TemplateSettings v-if="draw == 'settings'" ref="settings" :visible="visible" :current_round="current_round" :template="current_template" @create-game="create_game" @change-count="set_count" @to-questions="visible1" @back-to-templates="back_to_templates" @show-game="show_game"  @games="games" />
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
      user_templates: []
    }
  },
  methods:{
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
</style>