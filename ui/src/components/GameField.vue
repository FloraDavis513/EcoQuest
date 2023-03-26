<template>
    <img v-if="number_round == 1" src="@/assets/dice.png" alt="" id="dice" @click="generate_random_number">
    <div v-if="number_round == 1" id="generated_number">{{ current_number }}</div>
    <div v-if="number_round == 2 && !final" id="next_turn" @click="next_turn">Следующий ход</div>
    <div class="round">1 раунд</div>
    <div class="round" @click="next_round">2 раунд</div>
    <input id="price" @blur="save_price">
    <div class="round" @click="next_round">Финал</div>
    <img src="@/assets/start.png" alt="" id="start" @click="go_timer">
    <div id="timer">{{game.state.timer[0]}}:{{game.state.timer[1]}}:{{game.state.timer[2]}}</div>
    <img src="@/assets/start.png" id="question_start" @click="start_timer">
    <div id="question_timer">00:30</div>
    <FirstRoundField v-if="number_round == 1" ref="FirstRound" :logos="logos" :game_settings="game" :current_question="current_question" :turn="turn" @update-field="update_chip_positions"/>
    <SecondRoundField v-if="number_round != 1" :teams="calc_exist_team()" :game_settings="game" :question="current_question" @second-round-pos="init_second_round_pos" @second-round-question="sec_round_question" />
    <div class="chip" v-for="(player, index) in current_chip_poses" :key="index" :style="player">
      <img v-if="index == 0" :src="img_path + 'team_logo_' + String(Number(logos[0]) + 1) + '.svg'" style="width:100%;height:100%;">
      <img v-else-if="index == 1" :src="img_path + 'team_logo_' + String(Number(logos[1]) + 1) + '.svg'" style="width:100%;height:100%;">
      <img v-else-if="index == 3" :src="img_path + 'team_logo_' + String(Number(logos[2]) + 1) + '.svg'" style="width:100%;height:100%;">
      <img v-else-if="index == 2" :src="img_path + 'team_logo_' + String(Number(logos[3]) + 1) + '.svg'" style="width:100%;height:100%;">
    </div>
</template>

<script>
import FirstRoundField from './FirstRoundField.vue'
import SecondRoundField from './SecondRoundField.vue'
import { SRC_PATH } from '../common_const.js'

export default {
  name: 'GameField',
  props: ['teams', 'logos', 'timer', 'crit_timer', 'game_id', 'state', 'question', 'number_round', 'game'],
  data(){
    return {
        img_path: SRC_PATH,
        global_question_timer: null,
        global_timer: null,
        current_global_timer: null,
        current_global_save: null,
        current_time: null,
        is_paused: false,
        final_questions: [],
        semifinal_questions: [],
        second_tour_ids_question: [],
        second_round_price: 0,
        field_config: [ {name:'1'}, {name:'1'}, {name:'1'}, {name:'1'}, {name:'1'}, {name:'1'}, 
                        {name:'1'}, {name:'1'}, {name:'1'}, {name:'1'}, {name:'1'}, {name:'1'}, 
                        {name:'1'}, {name:'1'}, {name:'1'}, {name:'1'}, {name:'1'}, {name:'1'}, 
                        {name:'1'}, {name:'1'}, {name:'1'}, {name:'1'}, {name:'1'}, {name:'1'}, ],
        current_number: 0,
        next: 'Второй раунд',
        turn: null,
        tour: 1,
        second_round_tour: 0,
        second_round_states: [
            
        ],
        players: [
            { pos:'top:10.5%;left:31%;', color:'background:blue;' },
            { pos:'top:10.5%;left:68%;', color:'background:red;' },
            { pos:'top:83.5%;left:68%;', color:'background:lime;' },
            { pos:'top:83.5%;left:31%;', color:'background:cyan;' },
        ],
        current_chip_poses: [],
        circle: [],
        questions: {},
        second_round_questions: [],
        current_question_id: null,
        current_question: null,
        final: false
    }
  },
  components: {
    FirstRoundField,
    SecondRoundField
  },
  methods: {
    calc_exist_team()
    {
      let result = 0;
      for(let i = 0; i < this.teams.length; ++i)
        if( this.teams[i].length > 0 )
          ++result;
      return result;
    },
    async generate_random_number () {
      if( JSON.parse(localStorage.getItem('user')).role == 'player' )
        return;
        if(this.current_number == 0)  // Если ещё не начинали игру, то нужно инициализировать возможные позиции фишек и вопросы на каждом поле.
        {
            this.init_circle_chip_positions();
            this.init_questions();
        }
        if( !this.turn && this.turn != 0 ) // Если ход ещё не определён, то это ход 1 команды
          this.turn = 0;
            
        this.current_number = Math.floor(Math.random() * 6 + 1); // Бросаем кубик
        let number = this.current_number;
        if(this.tour == 1)  // Если это первый круг, то нужно сдвинуть фишку со стартовой позиции на основной круг.
        {
            this.current_chip_poses[this.turn] = this.circle[this.turn * (this.game.state.numFields / 4)];
            --number;
        }

        let current_pos;
        
        for(var i = 0; i < this.circle.length; ++i) // На каждом ходу сдвигаем фишку на выпавшее число.
        {
            // Делаем проверку, что не выходим за конец круга.
            // Сначала найдём, в какой части круга данная фишка. Затем перемещаем на новую позицию.
            if(this.circle[i] == this.current_chip_poses[this.turn] && i + number < this.circle.length)
            {
                this.current_chip_poses[this.turn] = this.circle[i + number];
                current_pos = i + number;
                break;
            }
            else if(this.circle[i] == this.current_chip_poses[this.turn] && i + number >= this.circle.length)
            {
                this.current_chip_poses[this.turn] = this.circle[-(this.circle.length - (number + i))];
                current_pos = -(this.circle.length - (number + i));
                break;
            }
        }

        this.current_question = {};
        this.current_question['current_pos'] = current_pos;
        this.current_question['question'] = this.questions[current_pos][0];
        console.log(this.questions[current_pos][0]);
        this.current_question['product'] = this.game.state.field_config[current_pos];
        [this.questions[current_pos][0], this.questions[current_pos][this.questions[current_pos].length - 1]] = [this.questions[current_pos][this.questions[current_pos].length - 1], this.questions[current_pos][0]];
        this.$emit('set-question', this.current_question, this.turn);

        this.$emit("update-poses", this.current_chip_poses);
        this.$emit("update-number", this.current_number);
        this.$emit("update-question", this.current_question);

        clearInterval(this.global_question_timer);
        document.getElementById("question_timer").innerText = '00:30';

        if(this.turn == this.current_chip_poses.length - 1)
        {
            this.turn = 0;
            ++this.tour;
        }
        else
            ++this.turn;
        },
    next_round () {
      if( JSON.parse(localStorage.getItem('user')).role == 'player' )
        return;
        if(this.number_round == 1 && this.second_round_price == 0)
        {
          alert("Количество баллов на второй тур не установлено");
          return;
        }
        
        if(this.number_round != 2)
        {
          this.$emit("update-price", this.second_round_price);
          this.$emit("update-number-round", 2);
          this.current_question = {};
          this.current_question['question'] = "За верный ответ играющей команде будет начислено " + String(this.second_round_price) + " баллов";
          this.current_question['product'] = null;
          this.$emit("update-question", this.current_question);
          this.$emit('next-round');
        }
        else
        {
          this.$emit("update-number-round", 3);
          this.$emit('end-game');
          return;
        }
            
        this.next = 'Завершить игру';
        },
    next_turn () {
      if( JSON.parse(localStorage.getItem('user')).role == 'player' )
        return;
        if(this.second_round_tour + 1 >= this.second_round_states[this.turn].length)
        {
            this.final = true;
            return;
        }
        let question = this.second_round_questions[this.second_round_tour * this.current_chip_poses.length + this.turn];
        this.current_question = {};
        this.current_question['question'] = question.text;
        this.current_question['product'] = question.product_name;
        this.current_chip_poses[this.turn] = this.second_round_states[this.turn][this.second_round_tour + 1];
        this.$emit('set-question', this.current_question, this.turn);
        this.$emit("update-poses", this.current_chip_poses);
        this.$emit("update-question", this.current_question);
        if(this.turn == this.current_chip_poses.length - 1)
          {
              this.turn = 0;
              ++this.tour;
              ++this.second_round_tour;
          }
          else
            ++this.turn;
    },
    go_timer () {
        if( JSON.parse(localStorage.getItem('user')).role == 'player' )
          return;

        if( !this.is_paused )
        {
          this.is_paused = true;
          clearInterval(this.current_global_timer);
          clearInterval(this.current_global_save);
          return;
        }
        this.is_paused = false;
        if( !this.current_time )
          this.current_time = new Date(2011, 0, 1, this.game.state.timer[0], this.game.state.timer[1], this.game.state.timer[2], 0);
        else
        {
          let tmp_timer = document.getElementById('timer').innerText;
          this.current_time = new Date(2011, 0, 1, tmp_timer.slice(0, 2), tmp_timer.slice(3, 5), tmp_timer.slice(6, 8), 0);
        }
          
        this.current_global_timer = setInterval(tick, 1000);
        this.current_global_save = setInterval(this.save_new_timer, 5000);
        let cur_timer = this.current_global_timer;
        let cur_saver = this.current_global_save;
        let crit_time = this.game.state.crit_timer;
        let time = this.current_time;
        function tick(){
            time = new Date(time - 1000);

            document.getElementById('timer').innerText = (time.getHours() < 10 ? '0' + String(time.getHours()) : String(time.getHours())) + ':' + (time.getMinutes() < 10 ? '0' + String(time.getMinutes()) : String(time.getMinutes())) + ':' + (time.getSeconds() < 10 ? '0' + String(time.getSeconds()) : String(time.getSeconds()));
            if(time.getHours() == Number(crit_time[0]) && time.getMinutes() == Number(crit_time[1]) && time.getSeconds() == Number(crit_time[2]))
                document.getElementById('timer').style.color = 'red';
                
            if(time.getHours() == 0 && time.getMinutes() == 0 && time.getSeconds() == 0)
            {
              clearInterval(cur_saver);
              clearInterval(cur_timer);
            }
        }
    },
    save_new_timer: function() {
        this.$emit("update-timer", document.getElementById('timer').innerText.slice(0, 2), document.getElementById('timer').innerText.slice(3, 5), document.getElementById('timer').innerText.slice(6, 8))
    },
    start_timer() {
      if( JSON.parse(localStorage.getItem('user')).role == 'player' )
        return;
        let field = document.getElementById("question_timer");
        if(field.innerText != '00:30')
            return;
        var secs = 30;
        this.global_question_timer = setInterval(tick,1000);
        let local_timer = this.global_question_timer;
        function tick(){
            (--secs);
            if(secs < 10)
                field.innerText = '00:0' + secs;
            else
                field.innerText = '00:' + secs;

            if( field.innerText == '00:05' )
              field.style.color = 'red';
            if(secs == 0)
            {
              clearInterval(local_timer);
              field.style.color = 'black';
            }
                
        }
    },
    custom_shuffle: function (array) {
      for (let i = array.length - 1; i > 0; i--) {
            let j = Math.floor(Math.random() * (i + 1));
            [array[i], array[j]] = [array[j], array[i]];
        }
    },
    update_chip_positions: async function () {
        if(this.current_number != 0 || this.number_round != 1)
          return;

        this.current_chip_poses.length = 0;
        this.teams.forEach((item, index) => {
            if(item.length > 0)
            {
              let start = document.getElementById("start_" + String(index + 1));
              this.current_chip_poses.push('top:' + start.getBoundingClientRect().top + 'px;left:' + start.getBoundingClientRect().left + "px;");
            }
        });

        this.$emit("update-poses", this.current_chip_poses);
    },
    init_circle_chip_positions: function () {
        this.circle.length = 0;
        for( let i = 0; i < this.game.state.field_config.length; ++i )
        {
          let field = document.getElementById("field_product_" + String(i));
          let top = field.getBoundingClientRect().top;
          let left = field.getBoundingClientRect().left;
          this.circle.push('left:' + String(left) + 'px;top:' + String(top) + 'px;');
        }
    },
    init_questions: function () {
      this.questions = [];
      for( let i = 0; i < this.game.state.field_config.length; ++i)
      {
          this.questions.push(this.game.state.questions_round_1[this.game.state.field_config[i].name]);
          this.custom_shuffle(this.questions[i]);
      }

    },
    update_questions: function (current_pos) {
      let mapping = new Map();
      mapping.set("TEXT", "Без выбора ответа").set("AUCTION", "Вопрос-аукцион").set("TEXT_WITH_ANSWERS", "С выбором ответа").set("MEDIA", "Вопрос с медиа фрагментом");
      
      let current_index = 0;
      for(let i = 0; i < this.circle.length; ++i)
      {
        if(this.circle[i] == current_pos)
        {
          current_index = i;
          break;
        }
      }
      if(this.field_config[current_index].questions.length > 1)
      {
        this.field_config[current_index].questions.shift();
        this.init_questions();
      }
    },
    replace_q: function () {
      this.current_question.question = this.questions[this.current_question.current_pos][0];
      [this.questions[this.current_question.current_pos][0], this.questions[this.current_question.current_pos][this.questions[this.current_question.current_pos].length - 1]] = [this.questions[this.current_question.current_pos][this.questions[this.current_question.current_pos].length - 1], this.questions[this.current_question.current_pos][0]];
      this.$emit('set-question', this.current_question, this.turn);
      this.$emit("update-poses", this.current_chip_poses);
      this.$emit("update-question", this.current_question);
    },
    replace_f: function () {
      let prev_turn = this.turn - 1;
      if( prev_turn ==  -1)
        prev_turn = 3;
      let current_pos;
      let number = 1;
      for(var i = 0; i < this.circle.length; ++i) // На каждом ходу сдвигаем фишку на выпавшее число.
      {
          // Делаем проверку, что не выходим за конец круга.
          // Сначала найдём, в какой части круга данная фишка. Затем перемещаем на новую позицию.
          if(this.circle[i] == this.current_chip_poses[prev_turn] && i + number < this.circle.length)
          {
              this.current_chip_poses[prev_turn] = this.circle[i + number];
              current_pos = i + number;
              break;
          }
          else if(this.circle[i] == this.current_chip_poses[prev_turn] && i + number >= this.circle.length)
          {
              this.current_chip_poses[prev_turn] = this.circle[-(this.circle.length - (number + i))];
              current_pos = -(this.circle.length - (number + i));
              break;
          }
      }
      console.log(current_pos);

      this.current_question = {};
      this.current_question['current_pos'] = current_pos;
      this.current_question['question'] = this.questions[current_pos][0];
      this.current_question['product'] = this.game.state.field_config[current_pos];
      [this.questions[current_pos][0], this.questions[current_pos][this.questions[current_pos].length - 1]] = [this.questions[current_pos][this.questions[current_pos].length - 1], this.questions[current_pos][0]];
      this.$emit('set-question', this.current_question, this.turn);

      this.$emit("update-poses", this.current_chip_poses);
      this.$emit("update-number", this.current_number);
      this.$emit("update-question", this.current_question);
    },
    save_price: function () {
      this.second_round_price = document.getElementById("price").value;
    },
    get_price: function () {
      return this.second_round_price;
    },
    calc_color_round_buttons: function ()
    {
        let list_round = document.getElementsByClassName("round");
        if(this.number_round == 1)
        {
          list_round[0].style = "background-color:rgba(44, 192, 70, 1);top:38%;";
          list_round[1].style = "background-color:rgba(188, 243, 197, 1); width:10%;left:80.5%;font-size:1.75vw;top:46.5%;height:4%;";
          list_round[2].style = "background-color:rgba(210, 241, 215, 1); width:7.5%;left:81.75%;font-size:1.35vw;top:54%;height:3%;";
        }
        if(this.number_round == 2)
        {
          list_round[0].style = "background-color:rgba(188, 243, 197, 1); width:10%;left:80.5%;font-size:1.75vw;top:38%;height:4%;";
          list_round[1].style = "background-color:rgba(44, 192, 70, 1);top:45.5%;";
          list_round[2].style = "background-color:rgba(188, 243, 197, 1); width:7.5%;left:81.75%;font-size:1.35vw;top:54%;height:3%;";
          document.getElementById("price").style = "left: 95.5%;";
        }
        if(this.number_round == 3)
        {
          
          list_round[0].style = "background-color:rgba(210, 241, 215, 1); width:7.5%;left:81.75%;font-size:1.35vw;top:38%;height:3%;";
          list_round[1].style = "background-color:rgba(188, 243, 197, 1);width:10%;left:80.5%;font-size:1.75vw;height:4%;top:45.5%;";
          list_round[2].style = "background-color:rgba(44, 192, 70, 1);top:54%;";
          document.getElementById("price").style = "left: 93%;top:46.3%;";
        }
    },
    init_second_round_pos: function ( second_round_poses )
    {
        this.turn = 0;
        this.second_round_tour = 0;
        this.second_round_states = second_round_poses;
        this.current_chip_poses.length = 0;
        for(let i = 0; i < this.calc_exist_team(); ++i)
          this.current_chip_poses.push(second_round_poses[i][0]);
    },
    sec_round_question: function ( second_round_questions )
    {
        this.second_round_questions = second_round_questions;
    },
  },
  beforeMount: async function () {
        if(this.game.state.current_chip_poses != null)
          this.current_chip_poses = this.game.state.current_chip_poses;
        if(this.game.state.current_question != null)
          this.current_question = this.game.state.current_question;
        if(this.game.state.current_number != null)
          this.current_number = this.game.state.current_number;
        // this.calc_color_round_buttons();
  },
  updated: function () {
      this.calc_color_round_buttons();
      this.update_chip_positions();
  },
  mounted: function () {
    this.update_chip_positions();
    this.calc_color_round_buttons();
    if(this.game.state.price != null)
      document.getElementById('price').value = this.game.state.price;
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
#timer{
    left: 37%;
    position: absolute;
    font-size: 2.2vw;
}

#start{
    width: 3%;
    height: 5%;
    left: 33%;
    position: absolute;
}

#dice{
    width: 7%;
    height: 11%;
    left: 5%;
    top: 44%;
    position: absolute;
}

img{
  transition: transform .25s ease;
}

img:hover {
  transform: scale(1.2); /* (150% zoom - Note: if the zoom is too large, it will go outside of the viewport) */
}

#generated_number{
    width: 3.3%;
    height: 6%;
    left: 13%;
    top: 47%;
    position: absolute;
    text-align: center;
    font-size: 2.8vw;
    line-height: 110%;
    background-color: white;
    border-radius: 0.3vw;
}

#price{
  position: absolute;
  text-align: center;
  width: 2%;
  height: 4%;
  left: 93%;
  top: 47%;
  font-size: 1vw;
}

#next_round{
    width: 15%;
    height: 5%;
    left: 79%;
    top: 47%;
    position: absolute;
    text-align: center;
    font-size: 2.2vw;
    line-height: 130%;
    color: black;
}

.round{
    width: 15%;
    height: 5%;
    left: 78%;
    top: 38%;
    position: absolute;
    text-align: center;
    font-size: 2.2vw;
    line-height: 130%;
    background: rgba(44, 192, 70, 1);
    background: rgba(188, 243, 197, 1);
    color: black;
    padding: 0.5% 1% 0.5% 1%;
    border-radius: 10px;
}

.chip{
    height: 4.6%;
    width: 2.3%;
    position: absolute;
    z-index: 3;
}

#next_turn{
    width: 15%;
    height: 5%;
    left: 2.5%;
    top: 45%;
    position: absolute;
    text-align: center;
    font-size: 1.5vw;
    line-height: 130%;
    background: white;
    color: black;
    padding: 0.5% 2% 0.5% 2%;
    border-radius: 0.5vw;
    border: solid 0.3vw rgba(44, 192, 70, 1);
}

#next_turn:hover {
    box-shadow: 0 0 10px 100px rgba(44, 192, 70, 1) inset;
}

#question_start{
    width: 3%;
    height: 5%;
    left: 58%;
    position: absolute;
}

#question_timer{
    left: 62%;
    position: absolute;
    font-size: 2.2vw;
}
</style>
