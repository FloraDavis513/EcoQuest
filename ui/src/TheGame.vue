<template>
    <TeamPanel ref="team_1" id="team_1" :helps="game.state.helps[0]" :themes="game.state.themes[0]" :logo="game.state.teams[0].team_logo" :name="game.state.teams[0].team_name" :players="game.state.teams[0].players" :team_number="'team_1'" :score="game.state.scores == null ? 0 : game.state.scores[0]" :tour="game.state.number_round != null ? game.state.number_round : 1" :question='current_question' @give-puzzle="give_puzzle" @give-score="give_score" @use-help="use_help" />
    <TeamPanel ref="team_2" id="team_2" :helps="game.state.helps[1]" :themes="game.state.themes[1]" :logo="game.state.teams[1].team_logo" :name="game.state.teams[1].team_name" :players="game.state.teams[1].players" :team_number="'team_2'" :score="game.state.scores == null ? 0 : game.state.scores[1]" :tour="game.state.number_round != null ? game.state.number_round : 1" :question='current_question' @give-puzzle="give_puzzle" @give-score="give_score" @use-help="use_help" />
    <TeamPanel ref="team_3" id="team_3" :helps="game.state.helps[2]" :themes="game.state.themes[2]" :logo="game.state.teams[2].team_logo" :name="game.state.teams[3].team_name" :players="game.state.teams[2].players" :team_number="'team_3'" :score="game.state.scores == null ? 0 : game.state.scores[2]" :tour="game.state.number_round != null ? game.state.number_round : 1" :question='current_question' @give-puzzle="give_puzzle" @give-score="give_score" @use-help="use_help" />
    <TeamPanel ref="team_4" id="team_4" :helps="game.state.helps[3]" :themes="game.state.themes[3]" :logo="game.state.teams[3].team_logo" :name="game.state.teams[2].team_name" :players="game.state.teams[3].players" :team_number="'team_4'" :score="game.state.scores == null ? 0 : game.state.scores[3]" :tour="game.state.number_round != null ? game.state.number_round : 1" :question='current_question' @give-puzzle="give_puzzle" @give-score="give_score" @use-help="use_help" />
    <GameField :game="game" :teams="[game.state.teams[0].players, game.state.teams[1].players, game.state.teams[2].players, game.state.teams[3].players]" :logos="[game.state.teams[0].team_logo, game.state.teams[1].team_logo, game.state.teams[2].team_logo, game.state.teams[3].team_logo]" :timer="timer ? timer : ['00', '00', '00']" :crit_timer="crit_timer ? crit_timer : ['00', '00', '00']" :game_id="game_id" :state="state" :question='current_question' :number_round="game.state.number_round != null ? game.state.number_round : 1" @set-question="set_question" @next-round="next_round" @end-game="end_game" @update-poses="update_poses" @update-number="update_current_number" @update-question="update_current_question" @update-price="update_price" @update-number-round="update_number_round" @update-timer="update_timer" ref="MainField" />
    <LeaderBoard @click="leaderboard_on = !leaderboard_on" v-if="leaderboard_on" :logos="[game.state.teams[0].team_logo, game.state.teams[1].team_logo, game.state.teams[2].team_logo, game.state.teams[3].team_logo]" :scores="[game.state.themes[0].filter(item => item.color != 'background:white;').length + game.state.scores[0], game.state.themes[1].filter(item => item.color != 'background:white;').length + game.state.scores[1], game.state.themes[2].filter(item => item.color != 'background:white;').length + game.state.scores[2], game.state.themes[3].filter(item => item.color != 'background:white;').length + game.state.scores[3]]"/>
</template>

<script>
import TeamPanel from './components/TeamPanel.vue'
import GameField from './components/GameField.vue'
import LeaderBoard from './components/LeaderBoard.vue'
import { SERVER_PATH } from './common_const.js'

export default {
  name: 'ManageMasters',
  props:['game_id', 'timer', 'crit_timer'],
  data(){
    return {
      leaderboard_on: false,
      unique_products: [],
      state: {},
      current_question: ["Для победы в игре:\n В 1 раунде необходимо собрать пазлы максимального количества цветов, верно отвечая на вопросы о продуктах ЭкоСистемы.\n\n Во 2 раунде - баллы, верно отвечая на вопросы по навыкам продаж, процессам или об ЭкоСистеме в целом.\n\n Полем управляет ведущий.\n Всем удачи!", 'Приветствуем Вас в игре по ЭкоСистеме Сбера', 'В 1 раунде необходимо собрать максимальное количество цветных пазлов, верно отвечая на вопросы о продуктах ЭкоСистемы.1 цвет = 1 верный ответ = 1 балл.\nЕсть 3 подсказки:\n- помощь других команд\n- замена вопроса\n- замена продукта (на следующий по часовой стрелке)\n\n  Во 2 раунде вопросы на проявление навыков, знаний процессов или общей категории. Количество баллов за верный ответ будет иным.\n\n  Над игровым полем 2 таймера - слева общее время игры, справа лимит для ответа на вопрос.\n\n  Полем управляет ведущий. Всем удачи!'],
      scores: [ 0, 0, 0, 0 ],
      tour: 1,
      turn: -1,
      second_turn: -1,
      products: [],
      game: null
    }
  },
  components: {
    TeamPanel,
    GameField,
    LeaderBoard
  }, 
  methods: {
    use_help: function (team, n) {
      if(n == 2)
        this.$refs.MainField.replace_q();
      else if(n == 3)
        this.$refs.MainField.replace_f();

      let mapping = new Map();
      mapping.set("team_1", 0).set("team_2", 1).set("team_3", 2).set("team_4", 3);
      this.game.state.helps[mapping.get(team)][n] = false;

      fetch(SERVER_PATH + '/game/update/stateAndQuestion', {
                  method: 'POST',
                  headers: {'Content-Type': 'application/json'},
                  body: JSON.stringify({gameId:this.game.gameId, state:JSON.stringify(this.game.state), currentQuestionId: null}),
              });
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
    give_puzzle: async function (team) {
      let mapping = new Map();
      mapping.set("team_1", 0).set("team_2", 1).set("team_3", 2).set("team_4", 3);
        this.game.state.themes[mapping.get(team)].forEach(item => {
          if(item.name == this.game.state.current_question.product.name)
            item.color = this.game.state.current_question.product.colour;
        });
        fetch(SERVER_PATH + '/game/update/stateAndQuestion', {
                  method: 'POST',
                  headers: {'Content-Type': 'application/json'},
                  body: JSON.stringify({gameId:this.game.gameId, state:JSON.stringify(this.game.state), currentQuestionId: null}),
              });
    },
    give_score: function (team) {
      let mapping = new Map();
      mapping.set("team_1", 0).set("team_2", 1).set("team_3", 2).set("team_4", 3);
      this.game.state.scores[mapping.get(team)] += Number(this.game.state.price);
      fetch(SERVER_PATH + '/game/update/stateAndQuestion', {
                  method: 'POST',
                  headers: {'Content-Type': 'application/json'},
                  body: JSON.stringify({gameId:this.game.gameId, state:JSON.stringify(this.game.state), currentQuestionId: null}),
              });
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
    set_unique: function () {
        if( this.game.state.themes != null )
          return;

        if( typeof(this.game.state) == 'string' )
            this.game.state = JSON.parse(this.game.state);
        this.game.state.themes = [];
        
        let unique_products = [];
        let unique_product_names = [];
        this.game.state.field_config.forEach(item => {
          if(!unique_product_names.includes(item.name))
          {
            unique_product_names.push(item.name);
            unique_products.push({name: item.name, colour: item.colour});
          }
        });
        let themes_sber = [];
        for( let i = 0; i < unique_products.length; ++i)
          themes_sber.push({name: unique_products[i].name, color: 'background:white;'});

        this.game.state.themes.push(JSON.parse(JSON.stringify(themes_sber)));
        this.game.state.themes.push(JSON.parse(JSON.stringify(themes_sber)));
        this.game.state.themes.push(JSON.parse(JSON.stringify(themes_sber)));
        this.game.state.themes.push(JSON.parse(JSON.stringify(themes_sber)));
        console.log(this.game.state);

        fetch(SERVER_PATH + '/game/update/stateAndQuestion', {
                  method: 'POST',
                  headers: {'Content-Type': 'application/json'},
                  body: JSON.stringify({gameId:this.game.gameId, state:JSON.stringify(this.game.state), currentQuestionId: null}),
              });
    },
    update_poses: function (new_poses) {
      this.game.state.current_chip_poses = new_poses;
      fetch(SERVER_PATH + '/game/update/stateAndQuestion', {
                    method: 'POST',
                    headers: {'Content-Type': 'application/json'},
                    body: JSON.stringify({gameId:this.game.gameId, state:JSON.stringify(this.game.state), currentQuestionId: null}),
                });
    },
    update_current_number: function (new_number) {
      this.game.state.current_number = new_number;
      fetch(SERVER_PATH + '/game/update/stateAndQuestion', {
                    method: 'POST',
                    headers: {'Content-Type': 'application/json'},
                    body: JSON.stringify({gameId:this.game.gameId, state:JSON.stringify(this.game.state), currentQuestionId: null}),
                });
    },
    update_current_question: function (new_question) {
      if( JSON.parse(localStorage.getItem('user')).role == 'player' )
        return;
      this.game.state.current_question = new_question;
      fetch(SERVER_PATH + '/game/update/stateAndQuestion', {
                    method: 'POST',
                    headers: {'Content-Type': 'application/json'},
                    body: JSON.stringify({gameId:this.game.gameId, state:JSON.stringify(this.game.state), currentQuestionId: null}),
                });
    },
    update_price: function (price) {
      this.game.state.price = price;
      this.game.state.scores = [0, 0, 0, 0];
      fetch(SERVER_PATH + '/game/update/stateAndQuestion', {
                    method: 'POST',
                    headers: {'Content-Type': 'application/json'},
                    body: JSON.stringify({gameId:this.game.gameId, state:JSON.stringify(this.game.state), currentQuestionId: null}),
                });
    },
    update_number_round: function (round) {
      this.game.state.number_round = round;
      fetch(SERVER_PATH + '/game/update/stateAndQuestion', {
                    method: 'POST',
                    headers: {'Content-Type': 'application/json'},
                    body: JSON.stringify({gameId:this.game.gameId, state:JSON.stringify(this.game.state), currentQuestionId: null}),
                });
      if(round == 3)
      {
        let state = this.game.state;
        let results = {};
        results.Teams = [];
        let active_teams = state.teams.filter(item => item.players.length != 0);
        for(let i = 0; i < active_teams.length; ++i)
        {
          if( state.teams[i].players.length == 0 )
            continue;
          let team_score = state.scores[i] + state.themes[i].filter(item => item.color != 'background:white;').length;
          results.Teams.push({Players: state.teams[i].players, Name:state.teams[i].team_name, Score: team_score});
        }
        results.Teams.sort((a, b) => a.Score - b.Score);
        for(let i = 0; i < results.Teams.length; ++i)
          results.Teams[i].Place = results.Teams.length - i;
        console.log(results);
        fetch(SERVER_PATH + '/statistic/create', {
                    method: 'POST',
                    headers: {'Content-Type': 'application/json'},
                    body: JSON.stringify({UserId:JSON.parse(localStorage.getItem('user')).userId, Duration:'01:00:00', Date:this.game.date, Results:JSON.stringify(results)}),
                });
      }
    },
    update_timer: function (hours, minutes, seconds) {
      
      this.game.state.timer = [hours, minutes, seconds];
      console.log(this.game.state)
      fetch(SERVER_PATH + '/game/update/stateAndQuestion', {
                    method: 'POST',
                    headers: {'Content-Type': 'application/json'},
                    body: JSON.stringify({gameId:this.game.gameId, state:JSON.stringify(this.game.state), currentQuestionId: null}),
                });
    },
    fill_helps: function () {
        if( this.game.state.helps != null )
          return;

        if( typeof(this.game.state) == 'string' )
            this.game.state = JSON.parse(this.game.state);
        this.game.state.helps = [[true, true, true, true], [true, true, true, true], [true, true, true, true], [true, true, true, true]];

        fetch(SERVER_PATH + '/game/update/stateAndQuestion', {
                  method: 'POST',
                  headers: {'Content-Type': 'application/json'},
                  body: JSON.stringify({gameId:this.game.gameId, state:JSON.stringify(this.game.state), currentQuestionId: null}),
              });
    },
  },
  mounted: function () {
    document.getElementById("html").style.backgroundColor = "rgb(210 , 210 , 210)";
    document.getElementById("html").style.fontFamily = "Menlo";
  },
  unmounted: function () {
  this.$nextTick(function () {
    document.getElementById("html").style.backgroundColor = "white";
    document.getElementById("html").style.fontFamily = "sans-serif";
  })
  },
  beforeCreate: async function () {
        await fetch(SERVER_PATH + "/game", {
                method: "GET",
                headers: {'Content-Type': 'application/json', "Authorization": "Bearer " + JSON.parse(localStorage.getItem('user')).authorizationToken}
        }).then(res => {
            if(res.status === 401)
                this.log_out();
        });

        await fetch(SERVER_PATH + "/game/get/" + String(this.game_id), {
                method: "GET",
                headers: {'Content-Type': 'application/json'}
                }).then( res => res.json() ).then( data => this.game = data );
        this.game.state = JSON.parse(this.game.state);
        console.log(this.game);
        if( this.game.state.themes == null )
          this.set_unique();
        if( this.game.state.helps == null )
          this.fill_helps();
          
        console.log(this.game);
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
