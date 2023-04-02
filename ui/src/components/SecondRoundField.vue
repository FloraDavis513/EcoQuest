<template>
  <div id="outer_field">
    <div id="question_area">
        <div v-if="question.product != null" id="question_header">
            {{question.product}} (2 раунд)
        </div>
        <div v-else id="question_header">
           2 раунд 
        </div>
        <div id="question_body"><div class="scroll">{{question.question}}</div></div>
    </div>
    <div v-for="(item, index) in teams" :key="index" style="width:100%;height:14%;margin-top:2%;">
        <div :id="'start_sec_round_' + index" class="questions start_sec_round">Старт</div>
        <div v-for="(q_item, q_index) in game_settings.state.second_round_config.filter(item => item != null).length - 1" :key="q_index" class="questions" :id="'second_round_question_' + String(index) + String(q_index)" ></div>
        <div :id="'finish_' + index" class="questions"><img src="@/assets/cup.png" id="cup"></div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'SecondRoundField',
  props: ['question', 'teams', 'game_settings'],
  data(){
    return {
        count_question: 5
    }
  },
  methods: {
  },
  mounted:  function () {
  this.$nextTick(async function () {
    var list = document.getElementsByClassName('questions');
    var bar_with_question = this.game_settings.state.second_round_config.filter(item => item != null).length;
    for (var i = 0; i < list.length; i++) {
        if( i % (bar_with_question + 1) == 0)
          continue;
        list[i].style.marginLeft = String( (81 - 12 * bar_with_question) / bar_with_question ) + '%';
    }

    let second_round_poses = [];
    let teams_number = this.teams;
    for( let j = 0; j < teams_number; ++j )
    {
      let poses_team = [];
      let start = document.getElementById('start_sec_round_' + String(j));
      poses_team.push('top:' + start.getBoundingClientRect().top + 'px;left:' + start.getBoundingClientRect().left + "px;");
      for( let i = 0; i < bar_with_question - 1; ++i )
      {
        var cur_field = document.getElementById("second_round_question_" + String(j) + String(i));
          poses_team.push('top:' + cur_field.getBoundingClientRect().top + 'px;left:' + cur_field.getBoundingClientRect().left + "px;");
      }
      let finish = document.getElementById('finish_' + String(j));
      poses_team.push('top:' + finish.getBoundingClientRect().top + 'px;left:' + finish.getBoundingClientRect().left + "px;");
      second_round_poses.push(poses_team);
    }

    let second_round_question = [];
    this.game_settings.state.second_round_config.filter(item => item != null).forEach(product => {
        let active_questions = [];
        product.allQuestions.forEach(question => {
          if(product.activeQuestions.includes(question.questionId))
          {
            question['product_name'] = product.name;
            active_questions.push(question);
          }
        });
        for (let i = active_questions.length - 1; i > 0; i--) {
          let j = Math.floor(Math.random() * (i + 1));
          [active_questions[i], active_questions[j]] = [active_questions[j], active_questions[i]];
        }
        for( let j = 0; j < teams_number; ++j )
          second_round_question.push(active_questions[j]);
    });

    if( JSON.parse(localStorage.getItem('user')).role == 'player' )
        return;
    this.$emit('second-round-question', second_round_question);
    this.$emit('second-round-pos', second_round_poses);
  })
  },
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
#question_area{
  border: 2px solid black;
  border-radius: 2.5vw;
  background: white;
  width: 100%;
  height: 35%;
}

.questions{
  margin-left: 1.5%;
  border: 2px solid black;
  border-radius: 0.75vw;
  background: white;
  width: 12%;
  height: 87%;
  float: left;
  box-shadow: 0.4vw 0.2vw 0.2vw gray;
}

#outer_field{
    height: 90%;
    width: 46%;
    position: absolute;
    left: 27%;
    top: 8%;
}

#question_header{
  margin-top: 2%;
  text-align: center;
  font-size: 1.75vw;
  color: green;
  height: 6%;
  width: 100%;
}

#question_body{
  text-align: justify;
  margin-top: 3%;
  margin-left: 5%;
  font-size: 1.5vw;
  height: 75%;
  width: 90%;
  text-indent: 5%;
}

.start_sec_round{
  font-size: 1.5vw;
  text-align: center;
  vertical-align: center;
  line-height: 5.1vw;
}

#cup{
  margin-left: 15%;
  margin-top: 15%;
  width: 70%;
  height: 70%;
}

.scroll{
    height: 98%;
    overflow: auto;
    -ms-overflow-style: none;
    scrollbar-width: none;
}

.scroll::-webkit-scrollbar {
    width: 0;
    height: 0;
}
</style>
