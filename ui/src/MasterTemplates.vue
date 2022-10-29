<template>
    <MasterHeader @logout="log_out" />
    <MasterMenu v-if="draw == 'settings'" @select-question= "visible1" @select-themes= "visible2" @create-game="create_game" @back-to-templates="back_to_templates" @save-tmpl-name="save_tmpl_name" @get-questions="get_questions" @check-delete="visible3" :template_name="current_template.text" :count="current_template_count" :questions="current_questions" :tmpl_id="current_template.id" />
    <TemplateList v-if="draw == 'grid'" :template_list="options" @select-template="select_template" @add-template="add_template" />
    <TemplateMenu v-if="draw == 'settings'" @round-1="round_1" @round-2="round_2" @games="games" />
    <TemplateSettings v-if="draw == 'settings'" ref="settings" :visible="visible" :products="current_template.products" :id="current_template.id" @change-count="set_count" @to-questions="visible1" @back-to-templates="back_to_templates" @show-game="show_game"  @games="games" />
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
      draw: 'grid',
      current_template: null,
      current_template_count: null,
      current_questions: null,
      products: [],
      options: [
            { text: 'Шаблон комнаты 1', value: '1',
            products: [
                { text: 'СберАптека',
                value: '3',
                color: 'color:white;background:red',
                count: 1,
                current_checked: 1,
                visible_question: true,
                questions: [
                    { id: '1', text: 'Вопрос 1', type: 'С выбором ответа', wording: 'Какой-то вопрос 1', Need_quest: false },
                    { id: '2', text: 'Вопрос 2', type: 'Без выбора ответа', wording: 'Какой-то вопрос 2', Need_quest: false },
                    { id: '3', text: 'Вопрос 3', type: 'Вопрос-аукцион', wording: 'Какой-то вопрос 3', Need_quest: false },
                    { id: '4', text: 'Вопрос 4', type: 'Вопрос с медиа фрагментом', wording: 'Какой-то вопрос 4', Need_quest: false }
                ]
                },
                { text: 'ДомКлик',
                value: '4',
                color: 'color:white;background:purple',
                count: 1,
                current_checked: 1,
                visible_question: true,
                questions: [
                    { id: '1', text: 'Вопрос 1', type: 'С выбором ответа', wording: 'Какой-то вопрос 5', Need_quest: false },
                    { id: '2', text: 'Вопрос 2', type: 'Без выбора ответа', wording: 'Какой-то вопрос 6', Need_quest: false },
                    { id: '3', text: 'Вопрос 3', type: 'Вопрос-аукцион', wording: 'Какой-то вопрос 7', Need_quest: false },
                    { id: '4', text: 'Вопрос 4', type: 'Вопрос с медиа фрагментом', wording: 'Какой-то вопрос 8', Need_quest: false }
                ]
                }
            ] },
            { text: 'Шаблон комнаты 2', value: '2', products: [] },
        ]
    }
  },
  methods:{
    round_1: function(){
        this.visible = 1;
    },
    round_2: function(){
        this.visible = 1;
    },
    games: function(){
        this.visible = 4;
    },
    show_game: function(){
        this.visible = 5;
    },
    log_out: function(){
        this.$emit('logout');
    },
    create_game: function(){
        this.$emit('create-game', this.current_template.id);
    },
    visible1: function () { // переключение на вкладку выбор вопросов
      this.visible = 1;
      this.$refs.settings.count_field_now();
    },
    visible2: function () { // переключение на вкладку выбор тем
      this.visible = 2
      this.$refs.settings.count_field_now();
    },
    visible3: function () { // переключение на вкладку выбор тем
      this.visible = 3
      this.$refs.settings.count_field_now();
    },
    select_template: function (tmpl) {
      this.draw = 'settings';
      this.current_template = tmpl;
    },
    back_to_templates: function () {
      this.visible = 1;
      let option_ref = this.options;
      option_ref.length = 0;
      fetch(SERVER_PATH + "/board/getAll", {
            method: "GET",
            headers: {'Content-Type': 'application/json'}
            }).then( res => res.json() ).then( data => data.forEach(function(item) {
                option_ref.push({id: item.id, text: item.name, num_fields: item.numFields})}) );
      this.draw = 'grid';
    },
    add_template: function () {
      this.options.push({ text: 'Новый шаблон', value: String(this.options.length), products: [] });
    },
    set_count: function (data) {
      this.current_template_count = data;
    },
    save_tmpl_name: function (data) {
      this.current_template.text = data;
    },
    get_questions: function () {
      this.current_questions = this.$refs.settings.get_product();
    },
  }, mounted: function () {
      let option_ref = this.options;
      option_ref.length = 0;
  this.$nextTick(function () {
    fetch(SERVER_PATH + "/board/getAll", {
            method: "GET",
            headers: {'Content-Type': 'application/json'}
            }).then( res => res.json() ).then( data => data.forEach(function(item) {
                option_ref.push({id: item.id, text: item.name, num_fields: item.numFields})}) );
  })
  }
  
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>