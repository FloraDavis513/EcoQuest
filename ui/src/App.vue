<template>
    <AuthWin v-if="current_page == 'Auth'" @login-master="login_master" @login-admin="login_admin"/>

    <AdminHeader v-if="current_page == 'Masters' || current_page == 'Fields' || current_page == 'Product'" />
    <NavigationButton v-if="current_page == 'Fields' || current_page == 'Masters'" :current_text="current_text" @go-to-fields="go_to_fields"/>

    <MasterList v-if="current_page == 'Masters'" ref="masters" @master-chosen="chose_master" :masters="masters" :master_chosen="master_chosen" />
    <MasterRequests v-if="current_page == 'Masters'" @go-back="go_back" @add-master="add_master" @remove-master="remove_master" :master_chosen="master_chosen" :selected="selected" :fio="fio" :login="login" />

    <FieldList v-if="current_page == 'Fields'" @add-field="add_field" @select-product="select_product" :products="products" />
    <AddFields v-if="current_page == 'Fields'" @close-add-field="close_add_field" @create-field="create_field"  :is_add_product="is_add_product" />

    <ProductMenu v-if="current_page == 'Product'" :selected_product="selected_product" @go-to-fields="go_to_fields" @set-draw="set_draw" />
    <QuestionsList v-if="current_page == 'Product'" @set-draw="set_draw" @delete-product="delete_product" @edit-product="edit_product" @edit-question="edit_question" :selected_product="selected_product" :draw="draw" :products="products" />

    <MasterHeader v-if="current_page == 'Templates' || current_page == 'Settings'"/>
    <TemplateList v-if="current_page == 'Templates'" />

    <TeamPanel v-if="current_page == 'Game'" ref="team_1" id="team_1" :team_number="'team_1'" :puzzle="puzzles[0]" :score="scores[0]" />
    <TeamPanel v-if="current_page == 'Game'" ref="team_2" id="team_2" :team_number="'team_2'" :puzzle="puzzles[1]" :score="scores[1]" />
    <TeamPanel v-if="current_page == 'Game'" ref="team_3" id="team_3" :team_number="'team_3'" :puzzle="puzzles[2]" :score="scores[2]" />
    <TeamPanel v-if="current_page == 'Game'" ref="team_4" id="team_4" :team_number="'team_4'" :puzzle="puzzles[3]" :score="scores[3]" />
    <GameField v-if="current_page == 'Game'" @set-question="set_question" @next-round="next_round" ref="MainField" />
    <PopUpQuestion v-if="current_page == 'Game'" :tour="tour" :question='current_question' @give-puzzle="give_puzzle" @give-score="give_score" />
</template>

<script>
import AdminHeader from './components/AdminHeader.vue'
import NavigationButton from './components/NavigationButton.vue'
import MasterList from './components/MasterList.vue'
import MasterRequests from './components/MasterRequests.vue'
import FieldList from './components/FieldList.vue'
import GameField from './components/GameField.vue'
import PopUpQuestion from './components/PopUpQuestion.vue'
import TeamPanel from './components/TeamPanel.vue'
import AddFields from './components/AddFields.vue'
import ProductMenu from './components/ProductMenu.vue'
import QuestionsList from './components/QuestionsList.vue'
import AuthWin from './components/AuthWin.vue'
import TemplateList from './components/TemplateList.vue'
import MasterHeader from './components/MasterHeader.vue'

export default {
  name: 'App',
  data(){
    return {
      current_page: 'Auth',
      current_text: 'Ведущие',
      master_chosen: false,
      selected: null,
      current_question: ['Для получения вопроса бросьте кубик', 'Тип вопроса', 'Название продукта'],
      puzzles: [ 0, 0, 0, 0 ],
      scores: [ 0, 0, 0, 0 ],
      tour: 1,
      fio: null,
      login: null,
      masters:[
        { text: 'Ведущий 1', value: '1' },
        { text: 'Ведущий 2', value: '2' },
        { text: 'Ведущий 3', value: '3' },
        { text: 'Ведущий 4', value: '4' },
        { text: 'Ведущий 5', value: '5' },
        { text: 'Ведущий 6', value: '6' },
        { text: 'Ведущий 7', value: '7' },
        { text: 'Ведущий 8', value: '8' },
        { text: 'Ведущий 9', value: '9' },
        { text: 'Ведущий 10', value: '10' },
        { text: 'Ведущий 11', value: '11' },
        { text: 'Ведущий 12', value: '12' },
        ],
      products: [
        { text: '1', value: '1', color:"color:black;font-size:500%;", 
          questions:[
            { id: '1', text: 'Вопрос 1', type: 'С выбором ответа', wording: 'Какой-то вопрос 1' }, 
            { id: '2', text: 'Вопрос 2', type: 'Без выбора ответа', wording: 'Какой-то вопрос 2' }, 
            { id: '3', text: 'Вопрос 3', type: 'Вопрос-аукцион', wording: 'Какой-то вопрос 3' }, 
            { id: '4', text: 'Вопрос 4', type: 'Вопрос с медиа фрагментом', wording: 'Какой-то вопрос 4' },
            ] 
        },
        { text: '2', value: '2', color:"color:black;font-size:500%;", 
          questions:[
            { id: '1', text: 'Вопрос 1', type: 'С выбором ответа', wording: 'Какой-то вопрос 5' }, 
            { id: '2', text: 'Вопрос 2', type: 'Без выбора ответа', wording: 'Какой-то вопрос 6' }, 
            { id: '3', text: 'Вопрос 3', type: 'Вопрос-аукцион', wording: 'Какой-то вопрос 7' }, 
            { id: '4', text: 'Вопрос 4', type: 'Вопрос с медиа фрагментом', wording: 'Какой-то вопрос 8' },
            ]  
        },
        { text: 'СберАптека', value: '3', color:"color:white;background:red", 
          questions:[
            { id: '1', text: 'Вопрос 1', type: 'С выбором ответа', wording: 'Какой-то вопрос 1' }, 
            { id: '2', text: 'Вопрос 2', type: 'Без выбора ответа', wording: 'Какой-то вопрос 2' }, 
            { id: '3', text: 'Вопрос 3', type: 'Вопрос-аукцион', wording: 'Какой-то вопрос 3' }, 
            { id: '4', text: 'Вопрос 4', type: 'Вопрос с медиа фрагментом', wording: 'Какой-то вопрос 4' },
            ] 
        },
        { text: 'ДомКлик', value: '4', color:"color:white;background:purple", 
          questions:[
            { id: '1', text: 'Вопрос 1', type: 'С выбором ответа', wording: 'Какой-то вопрос 5' }, 
            { id: '2', text: 'Вопрос 2', type: 'Без выбора ответа', wording: 'Какой-то вопрос 6' }, 
            { id: '3', text: 'Вопрос 3', type: 'Вопрос-аукцион', wording: 'Какой-то вопрос 7' }, 
            { id: '4', text: 'Вопрос 4', type: 'Вопрос с медиа фрагментом', wording: 'Какой-то вопрос 8' },
            ]  
        },
      ],
      is_add_product: false,
      selected_product: ['СберАптека', 'background:red'],
      draw: 'questions',
    }
  },
  components: {
    AdminHeader,
    NavigationButton,
    MasterList,
    MasterRequests,
    FieldList,
    GameField,
    PopUpQuestion,
    TeamPanel,
    AddFields,
    ProductMenu,
    QuestionsList,
    AuthWin,
    TemplateList,
    MasterHeader
  }, 
  methods: {
    go_to_fields: function () {
              if(this.current_page == 'Fields')
              {
                this.current_page = 'Masters';
                this.current_text = 'Игровые поля';
              }
              else if(this.current_page == 'Masters')
              {
                this.current_page = 'Fields';
                this.current_text = 'Ведущие';
              }
              else{
                 this.current_page = 'Fields';
                 this.current_text = 'Ведущие';
              }
            },
    set_question: function (question) {
      this.current_question = question;
    },
    give_puzzle: function (team, product) {
      if(team == 'team_1')
      {
        this.$refs.team_1.action(product, this.$refs.MainField.get_color(product));
        ++this.puzzles[0];
      }
        
      if(team == 'team_2')
      {
        this.$refs.team_2.action(product, this.$refs.MainField.get_color(product));
        ++this.puzzles[1];
      }
      if(team == 'team_3')
      {
        this.$refs.team_3.action(product, this.$refs.MainField.get_color(product));
        ++this.puzzles[2];
      }
      if(team == 'team_4')
      {
        this.$refs.team_4.action(product, this.$refs.MainField.get_color(product));
        ++this.puzzles[3];
      }
    },
    give_score: function (team, score) {
      if(team == 'team_1')
      {
        this.scores[0] += Number(score);
      }
        
      if(team == 'team_2')
      {
        this.scores[1] += Number(score);
      }
      if(team == 'team_3')
      {
        this.scores[2] += Number(score);
      }
      if(team == 'team_4')
      {
        this.scores[3] += Number(score);
      }
    },
    chose_master: function (master_chosen, selected) {
        this.master_chosen = master_chosen;
        this.selected = selected;
        this.fio = selected.getAttribute("name");
        this.login = selected.getAttribute("name");
        
    },
    go_back: function () {
        this.master_chosen = false;
        this.$refs.masters.unselect();
    },
    next_round: function () {
        this.tour = 2;
    },
    add_master: function(name, val){
      this.masters.push({text: name, value:val});
    },
    remove_master: function(value){
      this.masters = this.masters.filter(option => option.value != value);
      this.master_chosen = false;
      this.$refs.masters.unselect();
    },
    add_field: function(){
      this.is_add_product = true;
    },
    close_add_field: function(){
      this.is_add_product = false;
    },
    create_field: function(name, color){
      this.products.push({ text: name, value: this.products.lenght, color:"background:" + color, questions: [] });
    },
    select_product: function(name, color){
      if(name == '1' || name == '2')
        this.selected_product = [name, color + ';color:black;font-size:500%;padding-top: 5%;padding-bottom: 5%;'];
      else
        this.selected_product = [name, color + ';color:white;'];
      this.current_page = 'Product';
    },
    set_draw: function(draw){
      this.draw = draw;
    },
    delete_product: function(){
      this.products = this.products.filter(option => option.text != this.selected_product[0]);
      this.current_page = 'Fields';
    },
    edit_product: function(name, color){
      var old_name = this.selected_product[0];
      this.selected_product = [name, color];
      this.products.forEach(function(item) {
            if(item.text == old_name)
            {
              item.text = name;
              item.color = color;
            }
              
        });
    },
    edit_question: function(product, question_id, new_type, new_wording){
      this.products.forEach(function(item) {
            if(item.text == product)
            {
              item.questions.forEach(function(item) {
                if(item.id == question_id)
                {
                  item.type = new_type;
                  item.wording = new_wording;
                  return;
                }
                  
            });   
            }
        });
    },
    login_master: function(){
      this.current_page = 'Templates';
    },
    login_admin: function(){
      this.current_page = 'Fields';
    },
  },
  mounted: function () {
  this.$nextTick(function () {
    // Код, который будет запущен только после
    // отображения всех представлений
      document.getElementById('team_2').style.background = 'rgba(214, 28, 28, 0.68)';
      document.getElementById('team_3').style.background = 'rgba(28, 180, 214, 0.35)';
      document.getElementById('team_4').style.background = 'rgba(117, 205, 6, 0.65)';
  })
  
}
,
}
</script>

<style scoped>
html {
    height: 100%;
    width: 100%;
    overflow: auto;
    -ms-overflow-style: none;
    scrollbar-width: none;
    font-family: 'Roboto Light', sans-serif;
}

body {
    width: 99%;
    height: 90%;
}
#app{
  height: 100%;
    width: 100%;
}

#team_1{
  position: absolute;
  left: 1%;
}

#team_2{
  position: absolute;
  left: 74%;
}

#team_3{
  position: absolute;
  left: 1%;
  top: 63%;
}

#team_4{
  position: absolute;
   top: 63%;
  left: 74%;
}
</style>
