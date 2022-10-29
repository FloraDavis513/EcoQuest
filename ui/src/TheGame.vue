<template>
    <TeamPanel ref="team_1" id="team_1" :logo="teams_logos[0]" :name="teams_name[0]" :unique_products="unique_products" :players="teams ? teams[0] : []" :team_number="'team_1'" :puzzle="puzzles[0]" :score="scores[0]" :tour="tour" :question='current_question' @give-puzzle="give_puzzle" @give-score="give_score" @replace-question="replace_question" @replace-field="replace_field" />
    <TeamPanel ref="team_2" id="team_2" :logo="teams_logos[1]" :name="teams_name[1]" :unique_products="unique_products" :players="teams ? teams[1] : []" :team_number="'team_2'" :puzzle="puzzles[1]" :score="scores[1]" :tour="tour" :question='current_question' @give-puzzle="give_puzzle" @give-score="give_score" @replace-question="replace_question" @replace-field="replace_field" />
    <TeamPanel ref="team_3" id="team_3" :logo="teams_logos[2]" :name="teams_name[3]" :unique_products="unique_products" :players="teams ? teams[2] : []" :team_number="'team_3'" :puzzle="puzzles[2]" :score="scores[2]" :tour="tour" :question='current_question' @give-puzzle="give_puzzle" @give-score="give_score" @replace-question="replace_question" @replace-field="replace_field" />
    <TeamPanel ref="team_4" id="team_4" :logo="teams_logos[3]" :name="teams_name[2]" :unique_products="unique_products" :players="teams ? teams[3] : []" :team_number="'team_4'" :puzzle="puzzles[3]" :score="scores[3]" :tour="tour" :question='current_question' @give-puzzle="give_puzzle" @give-score="give_score" @replace-question="replace_question" @replace-field="replace_field" />
    <GameField :teams="teams" :logos="teams_logos" :timer="timer ? timer : ['00', '00', '00']" :crit_timer="crit_timer ? crit_timer : ['00', '00', '00']" :tmpl_id="tmpl_id" :state="state" :question='current_question' :number_round="tour" @set-question="set_question" @next-round="next_round" @set-config="set_config" @set-unique="set_unique" @end-game="end_game" ref="MainField" />
    <LeaderBoard @click="leaderboard_on = !leaderboard_on" v-if="leaderboard_on" :logos="teams_logos" :scores="[puzzles[0] + scores[0], puzzles[1] + scores[1], puzzles[2] + scores[2], puzzles[3] + scores[3]]"/>
</template>

<script>
import TeamPanel from './components/TeamPanel.vue'
import GameField from './components/GameField.vue'
import LeaderBoard from './components/LeaderBoard.vue'
import { SERVER_PATH } from './common_const.js'

export default {
  name: 'ManageMasters',
  props:['teams_logos', 'teams_name', 'teams', 'timer', 'crit_timer', 'tmpl_id'],
  data(){
    return {
      leaderboard_on: false,
      unique_products: [],
      state: {},
      current_question: ["Для победы в игре:\n В 1 раунде необходимо собрать пазлы максимального количества цветов, верно отвечая на вопросы о продуктах ЭкоСистемы.\n\n Во 2 раунде - баллы, верно отвечая на вопросы по навыкам продаж, процессам или об ЭкоСистеме в целом.\n\n Полем управляет ведущий.\n Всем удачи!", 'Приветствуем Вас в игре по ЭкоСистеме Сбера', 'В 1 раунде необходимо собрать максимальное количество цветных пазлов, верно отвечая на вопросы о продуктах ЭкоСистемы.1 цвет = 1 верный ответ = 1 балл.\nЕсть 3 подсказки:\n- помощь других команд\n- замена вопроса\n- замена продукта (на следующий по часовой стрелке)\n\n  Во 2 раунде вопросы на проявление навыков, знаний процессов или общей категории. Количество баллов за верный ответ будет иным.\n\n  Над игровым полем 2 таймера - слева общее время игры, справа лимит для ответа на вопрос.\n\n  Полем управляет ведущий. Всем удачи!'],
      puzzles: [ 0, 0, 0, 0 ],
      scores: [ 0, 0, 0, 0 ],
      tour: 1,
      turn: -1,
      second_turn: -1,
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
      ]
    }
  },
  components: {
    TeamPanel,
    GameField,
    LeaderBoard
  }, 
  methods: {
    replace_question: function () {
      this.$refs.MainField.replace_q();
    },
    replace_field: function () {
      this.$refs.MainField.replace_f();
    },
    end_game: function () {
      this.tour = 3;
      this.leaderboard_on = true;
    },
    set_question: function (question, turn) {
      this.current_question = question;
      this.turn = turn;
    },
    give_puzzle: function (team, product) {
      if(team == 'team_1')
      {
        if(this.$refs.team_1.check_color(product) != 'background:white;')
          return;
        this.$refs.team_1.action(product, this.$refs.MainField.get_color(product));
        ++this.puzzles[0];
      }
        
      if(team == 'team_2')
      {
        if(this.$refs.team_2.check_color(product) != 'background:white;')
          return;
        this.$refs.team_2.action(product, this.$refs.MainField.get_color(product));
        ++this.puzzles[1];
      }
      if(team == 'team_3')
      {
        if(this.$refs.team_3.check_color(product) != 'background:white;')
          return;
        this.$refs.team_3.action(product, this.$refs.MainField.get_color(product));
        ++this.puzzles[2];
      }
      if(team == 'team_4')
      {
        if(this.$refs.team_4.check_color(product) != 'background:white;')
          return;
        this.$refs.team_4.action(product, this.$refs.MainField.get_color(product));
        ++this.puzzles[3];
      }
      this.state.puzzles = this.puzzles;
      fetch(SERVER_PATH + "/game/chooseQuestion", {
                  method: "POST",
                  headers: {'Content-Type': 'application/json'},
                  body: JSON.stringify({questionId:1, questionType:"REGULAR", state:JSON.stringify(this.state)})
                  });
    },
    give_score: function (team) {
      if(team == 'team_1')
      {
        this.scores[0] += Number(this.$refs.MainField.get_price());
      }
        
      if(team == 'team_2')
      {
        this.scores[1] += Number(this.$refs.MainField.get_price());
      }
      if(team == 'team_3')
      {
        this.scores[2] += Number(this.$refs.MainField.get_price());
      }
      if(team == 'team_4')
      {
        this.scores[3] += Number(this.$refs.MainField.get_price());
      }
    },
    next_round: function () {
        this.tour = 2;
    },
    save_state: function (data) {
        if(data.state != 'game_begin')
        {
          this.state = JSON.parse(data.state);
          this.$refs.MainField.set_db_config(this.state.field_config);
        }
        else
          this.state = data.state;
    },
    set_config: function (config) {
      if(this.state == 'game_begin')
        this.state = {};
        this.state.field_config = config;
    },
    set_unique: function (unique_products) {
        this.unique_products = unique_products;
        this.$refs.team_1.fill_themes_sber(unique_products);
        this.$refs.team_2.fill_themes_sber(unique_products);
        this.$refs.team_3.fill_themes_sber(unique_products);
        this.$refs.team_4.fill_themes_sber(unique_products);
    },
  },
  mounted: function () {
  this.$nextTick(function () {
    fetch(SERVER_PATH + "/game/getAnswer", {
              method: "GET",
              headers: {'Content-Type': 'application/json'}
              }).then( res => res.json() ).then( data => this.save_state(data) );

    if(this.state == 'game_begin')
    {
      this.state.puzzles = this.puzzles;
      this.state.scores = this.scores;
      this.state.teams = this.teams;
      this.state.timer = this.timer;
      this.state.crit_timer = this.crit_timer;
      this.state.tmpl_id = this.tmpl_id;
      fetch(SERVER_PATH + "/game/chooseQuestion", {
                  method: "POST",
                  headers: {'Content-Type': 'application/json'},
                  body: JSON.stringify({questionId:1, questionType:"REGULAR", state:JSON.stringify(this.state)})
                  });
    }
    document.getElementById("html").style.backgroundColor = "rgb(210 , 210 , 210)";
    document.getElementById("html").style.fontFamily = "Menlo";
  })
  },
  unmounted: function () {
  this.$nextTick(function () {
    document.getElementById("html").style.backgroundColor = "white";
    document.getElementById("html").style.fontFamily = "sans-serif";
  })
  }
}
</script>

<style scoped>
@import url('http://fonts.cdnfonts.com/css/menlo');
#team_1{
  position: absolute;
  left: 1%;
}

#team_2{
  position: absolute;
  left: 77%;
}

#team_3{
  position: absolute;
  left: 1%;
  top: 63%;
}

#team_4{
  position: absolute;
   top: 63%;
  left: 77%;
}
</style>
