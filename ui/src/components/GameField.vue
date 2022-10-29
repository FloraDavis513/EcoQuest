<template>
    <img v-if="next=='Второй раунд'" src="@/assets/dice.png" alt="" id="dice" @click="generate_random_number">
    <div v-if="next=='Второй раунд'" id="generated_number">{{ current_number }}</div>
    <div v-if="next=='Завершить игру'" id="next_turn" @click="next_turn">Следующий ход</div>
    <div class="round">1 раунд</div>
    <div class="round" @click="next_round">2 раунд</div>
    <input id="price" @blur="save_price">
    <div class="round" @click="next_round">Финал</div>
    <img src="@/assets/start.png" alt="" id="start" @click="go_timer">
    <div id="timer">{{timer[0]}}:{{timer[1]}}:{{timer[2]}}</div>
    <img src="@/assets/start.png" id="question_start" @click="start_timer">
    <div id="question_timer">00:30</div>
    <FirstRoundField v-if="number_round == 1" ref="FirstRound" :logos="logos" :current_template="current_template" :field_config="field_config" :question="question" :turn="turn" />
    <SecondRoundField v-if="number_round != 1" :teams="calc_exist_team()" :current_template="current_template" :field_config="field_config" :question="question" @second-round-pos="init_second_round_pos" />
    <div class="chip" v-for="(player, index) in players" :key="index" :style="player.pos">
      <img v-if="index == 0" :src="img_path + 'team_logo_' + String(Number(logos[0]) + 1) + '.svg'" style="width:100%;height:100%;">
      <img v-else-if="index == 1" :src="img_path + 'team_logo_' + String(Number(logos[1]) + 1) + '.svg'" style="width:100%;height:100%;">
      <img v-else-if="index == 3" :src="img_path + 'team_logo_' + String(Number(logos[2]) + 1) + '.svg'" style="width:100%;height:100%;">
      <img v-else-if="index == 2" :src="img_path + 'team_logo_' + String(Number(logos[3]) + 1) + '.svg'" style="width:100%;height:100%;">
    </div>
</template>

<script>
import FirstRoundField from './FirstRoundField.vue'
import SecondRoundField from './SecondRoundField.vue'
import { SERVER_PATH } from '../common_const.js'
import { SRC_PATH } from '../common_const.js'

export default {
  name: 'GameField',
  props: ['teams', 'logos', 'timer', 'crit_timer', 'tmpl_id', 'state', 'question', 'number_round'],
  data(){
    return {
        img_path: SRC_PATH,
        global_question_timer: null,
        global_timer: null,
        current_global_timer: null,
        current_time: null,
        is_paused: false,
        final_questions: [],
        semifinal_questions: [],
        second_tour_ids_question: [],
        second_round_price: 0,
        unique_products: [],
        current_template: {numFields: 16},
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
        circle: [
            'top:8.5%;left:40.1%;',
            'top:8.5%;left:46.5%;',
            'top:8.5%;left:52.8%;',
            'top:8.5%;left:59.2%;',
            'top:28.3%;left:68.9%;',
            'top:41%;left:68.9%;',
            'top:53.5%;left:68.9%;',
            'top:66.5%;left:68.9%;',
            'top:85.6%;left:59.3%;',
            'top:85.6%;left:52.9%;',
            'top:85.6%;left:46.5%;',
            'top:85.6%;left:40%;',
            'top:66.5%;left:30.3%;',
            'top:53.7%;left:30.3%;',
            'top:41.3%;left:30.3%;',
            'top:28.5%;left:30.3%;',
        ],
        questions: {
            'top:8.5%;left:40.1%;': ['Почему мне надо пользоваться приложением СберЗвук? Назовите минимум 3 преимущества', 'Без выбора ответа', 'СберЗвук'],
            'top:8.5%;left:46.5%;': ['В каком году Работа.ру вошла в Экосистему Сбера?\nВарианты ответа: 2019, 2020, 2017', 'С выбором ответа', 'РаботаРу'],
            'top:8.5%;left:52.8%': ['Какое кол-во контента в Окко в подписке Лайт?', 'Вопрос-аукцион', 'Окко'],
            'top:8.5%;left:59.2%;': ['Сколько ГБ в облачном хранении СберДиск Вы можете получить бесплатно?', 'Вопрос-аукцион', 'СберДиск'],
            'top:28.3%;left:68.9%;': ['Что такое СберЕАптека?', 'Без выбора ответа', 'СберЕАптека'],
            'top:41%;left:68.9%;': ['Какой период дается клиенту на поиск обьекта недвижимости после одобрения заявки на ипотеку?\nВарианты ответа: 90 дней, 30 календарных дней, 100 дней', 'С выбором ответа', 'ДомКлик'],
            'top:53.5%;left:68.9%;': ['Каким образом можно связываться с врачом?', 'Без выбора ответа', 'СберЗдоровье'],
            'top:66.5%;left:68.9%;': ['На базе какого маркетплейса создан СберМегаМаркет?', 'Без выбора ответа', 'СберМегаМаркет'],

            'top:85.6%;left:59.3%;': ['Доставляет ли Самокат еду в горячем виде?\nВарианты ответа: Да, нет', 'С выбором ответа', 'Самокат'],
            'top:85.6%;left:52.9%;': ['Сколько ресторанов-партнёров в сервисе DeliveryClub?', 'Вопрос-аукцион', 'ДеливериКлаб'],
            'top:85.6%;left:46.5%;': ['В какие сроки возможен возврат доставленных продуктов?', 'Без выбора ответа', 'СберМаркет'],
            'top:85.6%;left:40%;': ['Что представляет собой SberBox?', 'Без выбора ответа', 'СберБокс'],
            'top:66.5%;left:30.3%;': ['Как оплатить услуги связи назовите, минимум 5 способов?', 'Без выбора ответа', 'СберМобайл'],
            'top:53.7%;left:30.3%;': ['Какое количество сервисов доступно по подписке?', 'Без выбора ответа', 'СберПрайм+'],
            'top:41.3%;left:30.3%;': ['Сколько стоит подписка?', 'Без выбора ответа', 'СберПрайм'],
            'top:28.5%;left:30.3%;': ['Каким образом заказать такси, если закончился трафик?', 'Без выбора ответа', 'СитиМобил'],
        },
        second_round_questions: [
            'отработать сомнение клиента, которому предлагается СберМобайл: "У меня безлимитный интернет"?',
            'отработать сомнение клиента, которому предлагается СберМобайл: "У меня безлимитный интернет"?',
            'отработать сомнение клиента, которому предлагается СберМобайл: "У меня безлимитный интернет"?',
            'отработать сомнение клиента, которому предлагается СберМобайл: "У меня безлимитный интернет"?',
            'Команда рассказывает позитивный опыт или лайфхак по продукту',
            'Необходимо назвать минимум одну жизненную ситуацию, в которой выпавший продукт будет особенно ценен и выгоден клиенту',
            'Команда рассказывает позитивный опыт или лайфхак по продукту',
            'Необходимо назвать минимум одну жизненную ситуацию, в которой выпавший продукт будет особенно ценен и выгоден клиенту',
        ],
        current_question_id: null,
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
    generate_random_number () { 
        if(this.current_number == 0)
        {
            this.init_circle_chip_positions();
            this.init_questions();
        }
        if( !this.turn && this.turn != 0 )
          this.turn = 0;
            
        this.current_number = Math.floor(Math.random() * 6 + 1);
        let number = this.current_number;
        if(this.tour == 1)
        {
            this.players[this.turn].pos = this.circle[this.turn * (this.current_template.numFields / 4)];
            --number;
        }
           
        for(var i = 0; i < this.circle.length; ++i)
        {
            if(this.circle[i] == this.players[this.turn].pos && i + number < this.circle.length)
            {
                this.players[this.turn].pos = this.circle[i + number];
                break;
            }
            else if(this.circle[i] == this.players[this.turn].pos && i + number >= this.circle.length)
            {
                this.players[this.turn].pos = this.circle[-(this.circle.length - (number + i))];
                break;
            }
        }
        
        this.$emit('set-question', this.questions[this.players[this.turn].pos], this.turn);
        let cur_q = {};
        cur_q.current_question = this.questions[this.players[this.turn].pos];

        fetch(SERVER_PATH + "/game/chooseQuestion", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({questionId:this.questions[this.players[this.turn].pos][3], questionType:"REGULAR", state: cur_q.current_question[0]})}) 
        this.update_questions(this.players[this.turn].pos);
        this.$refs.FirstRound.set_current_turn(this.turn);

        clearInterval(this.global_question_timer);
        document.getElementById("question_timer").innerText = '00:30';

        if(this.turn == this.players.length - 1)
        {
            this.turn = 0;
            ++this.tour;
        }
        else
            ++this.turn;
        },
    next_round () {
        if(this.number_round == 1 && this.second_round_price == 0)
        {
          alert("Количество баллов на второй тур не установлено");
          return;
        }
        this.$emit('set-question', ["За верный ответ играющей команде будет начислено " + String(this.second_round_price) + " баллов", '', '2 раунд'], this.second_round_tour - 1);
        this.second_round_questions.length = 0;
        this.second_tour_ids_question.length = 0;
        for( let i = 0; i < this.semifinal_questions.length; ++i )
        {
          this.second_round_questions.push(this.semifinal_questions[i].text);
          this.second_tour_ids_question.push(this.semifinal_questions[i].id);
        }
          
        for( let i = 0; i < this.final_questions.length; ++i )
        {
          this.second_round_questions.push(this.final_questions[i].text);
          this.second_tour_ids_question.push(this.final_questions[i].id);
        }
          
        if(this.number_round != 2)
        {
            this.players = [
            { pos:'top:42%;left:27.8%;', color:'background:blue;' },
            { pos:'top:56.5%;left:27.8%;', color:'background:red;' },
            { pos:'top:71%;left:27.8%;', color:'background:lime;' },
            { pos:'top:85.5%;left:27.8%;', color:'background:cyan;' },
            ];
            this.$emit('next-round');
        }
        else
        {
          this.$emit('end-game');
          return;
        }
            
        this.next = 'Завершить игру';  
        document.getElementById('next_round').innerText = '2 раунд';
        },
    next_turn () {
        if(this.second_round_tour + 1 == 2 * this.players.length)
        {
            this.next = 'Конец';
            this.players = this.second_round_states[this.second_round_tour++];
            this.$emit('set-question', [this.second_round_questions[this.second_round_tour - 1], '2 раунд', this.second_round_tour <= this.players.length ? 'Полуфинал' : 'Финал'], this.second_round_tour - 1);
            return;
        }
       this.players = this.second_round_states[this.second_round_tour++];
       this.$emit('set-question', [this.second_round_questions[this.second_round_tour - 1], '2 раунд', this.second_round_tour <= this.players.length ? 'Полуфинал' : 'Финал'], this.second_round_tour - 1);
       fetch(SERVER_PATH + "/game/chooseQuestion", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({questionId:this.second_tour_ids_question[this.second_round_tour - 1], questionType:"REGULAR", state:this.second_round_questions[this.second_round_tour - 1]})}) 
    },  
    get_color (product) {
        for(var i = 0; i < this.field_config.length; ++i)
        {
            if(this.field_config[i].name == product)
                return this.field_config[i].colour; 
        }
    },
    go_timer () {
        if( document.getElementById('timer').innerText != String(this.timer[0]) + ':' + String(this.timer[1]) + ':' + String(this.timer[2]) && !this.is_paused )
        {
          this.is_paused = true;
          clearInterval(this.current_global_timer);
          return;
        }
        this.is_paused = false;
        if( !this.current_time )
          this.current_time = new Date(2011, 0, 1, this.timer[0], this.timer[1], this.timer[2], 0);
        else
        {
          let tmp_timer = document.getElementById('timer').innerText;
          this.current_time = new Date(2011, 0, 1, tmp_timer.slice(0, 2), tmp_timer.slice(3, 5), tmp_timer.slice(6, 8), 0);
        }
          
        this.current_global_timer = setInterval(tick,1000);
        let cur_timer = this.current_global_timer;
        let crit_time = this.crit_timer;
        let time = this.current_time;
        function tick(){
            time = new Date(time - 1000);

            document.getElementById('timer').innerText = (time.getHours() < 10 ? '0' + String(time.getHours()) : String(time.getHours())) + ':' + (time.getMinutes() < 10 ? '0' + String(time.getMinutes()) : String(time.getMinutes())) + ':' + (time.getSeconds() < 10 ? '0' + String(time.getSeconds()) : String(time.getSeconds()));
            if(time.getHours() == Number(crit_time[0]) && time.getMinutes() == Number(crit_time[1]) && time.getSeconds() == Number(crit_time[2]))
                document.getElementById('timer').style.color = 'red';
                
            if(time.getHours() == 0 && time.getMinutes() == 0 && time.getSeconds() == 0)
                clearInterval(cur_timer);
        }
    },
    start_timer() {
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
    save_template_product: function (data) {
      this.current_template = data;
      let ar_ref = this.field_config;
      ar_ref.length = 0;
      this.current_template.products.forEach(element => {
          if(element.name == "Финал")
            element.questions.forEach(question => {this.final_questions.push(question);} )
          if(element.name == "Полуфинал")
            element.questions.forEach(question => {this.semifinal_questions.push(question);} )
          if(element.numOfRepeating > 0)
            this.unique_products.push(element);
          for( let i = 0; i < element.numOfRepeating; ++i )
            ar_ref.push(element);
      });
      this.custom_shuffle(this.field_config);
      this.field_config.forEach(element => {
          this.custom_shuffle(element.questions);
      });
      this.set_field_config();
    },
    custom_shuffle: function (array) {
      for (let i = array.length - 1; i > 0; i--) {
            let j = Math.floor(Math.random() * (i + 1));
            [array[i], array[j]] = [array[j], array[i]];
        }
    },
    init_chip_positions: function () {
        this.players.length = 0;
        let body = document.body;
        let body_bottom = body.getBoundingClientRect().bottom;
        let body_right = body.getBoundingClientRect().right;

        let start_one = document.getElementById("start_one");
        let top = start_one.getBoundingClientRect().top;
        let left = start_one.getBoundingClientRect().left;
        if(this.teams[0].length > 0)
          this.players.push({pos: 'left:' + String(left/body_right * 100) + '%;top:' + String(top/body_bottom * 100) + '%;'});
      

        let start_two = document.getElementById("start_two");
        top = start_two.getBoundingClientRect().top;
        left = start_two.getBoundingClientRect().left;
        if(this.teams[1].length > 0)
          this.players.push({pos: 'left:' + String(left/body_right * 100) + '%;top:' + String(top/body_bottom * 100) + '%;'});

        let start_three = document.getElementById("start_three");
        top = start_three.getBoundingClientRect().top;
        left = start_three.getBoundingClientRect().left;
        if(this.teams[3].length > 0)
          this.players.push({pos: 'left:' + String(left/body_right * 100) + '%;top:' + String(top/body_bottom * 100 - 3) + '%;'});

        let start_four = document.getElementById("start_four");
        top = start_four.getBoundingClientRect().top;
        left = start_four.getBoundingClientRect().left;
        if(this.teams[2].length > 0)
          this.players.push({pos: 'left:' + String(left/body_right * 100) + '%;top:' + String(top/body_bottom * 100 - 3) + '%;'});

    },
    init_circle_chip_positions: function () {
        let body = document.body;
        let body_bottom = body.getBoundingClientRect().bottom;
        let body_right = body.getBoundingClientRect().right;
        let ar = this.circle;
        ar.length = 0;
        for( let i = 0; i < this.field_config.length; ++i )
        {
          let field = document.getElementById("field_product_" + String(i));
          let top = field.getBoundingClientRect().top;
          let left = field.getBoundingClientRect().left;
          if( i < this.current_template.numFields / 4)
            ar.push('left:' + String(left/body_right * 100) + '%;top:' + String(top/body_bottom * 100) + '%;');
          else if( i < this.current_template.numFields / 2)
            ar.push('left:' + String(left/body_right * 100) + '%;top:' + String(top/body_bottom * 100 - 0.5*(i - this.current_template.numFields / 4)) + '%;');
          else if( i < 3 * this.current_template.numFields / 4)
            ar.push('left:' + String(left/body_right * 100) + '%;top:' + String(top/body_bottom * 100 - 3) + '%;');
          else if( i < this.current_template.numFields)
            ar.push('left:' + String(left/body_right * 100) + '%;top:' + String(top/body_bottom * 100 - 3 + 0.5*(i - 3*this.current_template.numFields / 4)) + '%;');
        }
    },
    init_questions: function () {
      let mapping = new Map();
      mapping.set("TEXT", "без выбора ответа").set("AUCTION", "вопрос-аукцион").set("TEXT_WITH_ANSWERS", "с выбором ответа").set("MEDIA", "вопрос с медиа фрагментом");
      this.questions = {};
      for( let i = 0; i < this.field_config.length; ++i)
      {
        this.questions[this.circle[i]] = [this.field_config[i].questions[0].text, mapping.get(this.field_config[i].questions[0].questionType), this.field_config[i].name, this.field_config[i].questions[0].id];
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
      let prev_turn = this.turn - 1;
      if( prev_turn ==  -1)
        prev_turn = 3;
      this.$emit('set-question', this.questions[this.players[prev_turn].pos], prev_turn);
      let cur_q = {};
        cur_q.current_question = this.questions[this.players[prev_turn].pos];

        fetch(SERVER_PATH + "/game/chooseQuestion", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({questionId:this.questions[this.players[prev_turn].pos][3], questionType:"REGULAR", state: cur_q.current_question[0]})}) 
        this.update_questions(this.players[prev_turn].pos);
        this.$refs.FirstRound.set_current_turn(prev_turn);
    },
    replace_f: function () {
      let prev_turn = this.turn - 1;
      if( prev_turn ==  -1)
        prev_turn = 3;
      let number = 1;
      for(var i = 0; i < this.circle.length; ++i)
        {
            if(this.circle[i] == this.players[prev_turn].pos && i + number < this.circle.length)
            {
                this.players[prev_turn].pos = this.circle[i + number];
                break;
            }
            else if(this.circle[i] == this.players[prev_turn].pos && i + number >= this.circle.length)
            {
                this.players[prev_turn].pos = this.circle[-(this.circle.length - (number + i))];
                break;
            }
        }
        
        this.$emit('set-question', this.questions[this.players[prev_turn].pos], prev_turn);
        let cur_q = {};
        cur_q.current_question = this.questions[this.players[prev_turn].pos];

        fetch(SERVER_PATH + "/game/chooseQuestion", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({questionId:this.questions[this.players[prev_turn].pos][3], questionType:"REGULAR", state: cur_q.current_question[0]})}) 
        this.update_questions(this.players[prev_turn].pos);
        this.$refs.FirstRound.set_current_turn(prev_turn);
    },
    set_field_config: function () {
      this.$emit('set-config', this.field_config);
      this.$emit('set-unique', this.unique_products);
    },
    set_db_config: function (config) {
      this.current_template.numFields = config.length;
      this.field_config = config;
    },
    save_price: function () {
      this.second_round_price = document.getElementById("price").value;
    },
    get_price: function () {
      return this.second_round_price;
    },
    sleep: function (milliseconds) {
            var start = new Date().getTime();
            for (var i = 0; i < 1e7; i++) {
                if ((new Date().getTime() - start) > milliseconds){
                break;
                }
            }
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
    init_second_round_pos: function ( ar, start, middle, finish )
    {
        this.second_round_states.length = 0;
        this.players.length = 0;
        var poses = [{pos: 'top:42%;left:27.8%;'}, {pos: 'top:56.5%;left:27.8%;'}, {pos: 'top:71%;left:27.8%;'}, {pos: 'top:85.5%;left:27.8%;'}]
        var uses_pos = 0;
        for(var i = 0; i < this.teams.length; ++i)
        {
          if(this.teams[i].length > 0)
            this.players.push(poses[uses_pos++]);
        }
        let current_poses = [ {pos: 'top:42%;left:' + String(middle) + '%;'},
                              {pos: 'top:56.5%;left:' + String(start) + '%;'},
                              {pos: 'top:71%;left:' + String(start) + '%;'},
                              {pos: 'top:85.5%;left:' + String(start) + '%;'}];
        this.second_round_states.push(current_poses.slice(0, this.players.length));
        current_poses = [ {pos: 'top:42%;left:' + String(middle) + '%;'},
                          {pos: 'top:56.5%;left:' + String(middle) + '%;'},
                          {pos: 'top:71%;left:' + String(start) + '%;'},
                          {pos: 'top:85.5%;left:' + String(start) + '%;'}];
        if(this.players.length > 1)
          this.second_round_states.push(current_poses.slice(0, this.players.length));
        current_poses = [ {pos: 'top:42%;left:' + String(middle) + '%;'},
                          {pos: 'top:56.5%;left:' + String(middle) + '%;'},
                          {pos: 'top:71%;left:' + String(middle) + '%;'},
                          {pos: 'top:85.5%;left:' + String(start) + '%;'}];
        if(this.players.length > 2)
          this.second_round_states.push(current_poses.slice(0, this.players.length));
        current_poses = [ {pos: 'top:42%;left:' + String(middle) + '%;'},
                          {pos: 'top:56.5%;left:' + String(middle) + '%;'},
                          {pos: 'top:71%;left:' + String(middle) + '%;'},
                          {pos: 'top:85.5%;left:' + String(middle) + '%;'}];
        if(this.players.length > 3)
          this.second_round_states.push(current_poses.slice(0, this.players.length));

        current_poses = [ {pos: 'top:42%;left:' + String(finish) + '%;'},
                          {pos: 'top:56.5%;left:' + String(middle) + '%;'},
                          {pos: 'top:71%;left:' + String(middle) + '%;'},
                          {pos: 'top:85.5%;left:' + String(middle) + '%;'}];
        this.second_round_states.push(current_poses.slice(0, this.players.length));
        current_poses = [ {pos: 'top:42%;left:' + String(finish) + '%;'},
                          {pos: 'top:56.5%;left:' + String(finish) + '%;'},
                          {pos: 'top:71%;left:' + String(middle) + '%;'},
                          {pos: 'top:85.5%;left:' + String(middle) + '%;'}];
        if(this.players.length > 1)
          this.second_round_states.push(current_poses.slice(0, this.players.length));
        current_poses = [ {pos: 'top:42%;left:' + String(finish) + '%;'},
                          {pos: 'top:56.5%;left:' + String(finish) + '%;'},
                          {pos: 'top:71%;left:' + String(finish) + '%;'},
                          {pos: 'top:85.5%;left:' + String(middle) + '%;'}];
        if(this.players.length > 2)
          this.second_round_states.push(current_poses.slice(0, this.players.length));
        current_poses = [ {pos: 'top:42%;left:' + String(finish) + '%;'},
                          {pos: 'top:56.5%;left:' + String(finish) + '%;'},
                          {pos: 'top:71%;left:' + String(finish) + '%;'},
                          {pos: 'top:85.5%;left:' + String(finish) + '%;'}];
        if(this.players.length > 3)
          this.second_round_states.push(current_poses.slice(0, this.players.length));
    },
  },
  mounted: function () {
      let id = this.tmpl_id;
  this.$nextTick(function () {
    if(id)
      fetch(SERVER_PATH + "/board/get/" + String(id), {
                method: "GET",
                headers: {'Content-Type': 'application/json'}
                }).then( res => res.json() ).then( data => this.save_template_product(data) );
                this.init_chip_positions();
    this.calc_color_round_buttons();
  })
  },
  updated: function () {
      this.calc_color_round_buttons();
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
