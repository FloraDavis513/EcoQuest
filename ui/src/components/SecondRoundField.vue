<template>
  <div id="outer_field">
    <div id="question_area">
        <div v-if="question[2] != '2 раунд'" id="question_header">
            {{question[2]}} ({{question[1]}})
        </div>
        <div v-else id="question_header">
           {{question[2]}} 
        </div>
        <div id="question_body"><div class="scroll">{{question[0]}}</div></div>
    </div>
    <div v-for="(item, index) in teams" :key="index" style="width:100%;height:14%;margin-top:2%;">
        <div id="start_sec_round" class="questions">Старт</div>
        <div v-for="(q_item, q_index) in count_question" :key="q_index" class="questions" :id="'second_round_question_' + String(index) + String(q_index)" ></div>
        <div id="finish" class="questions"><img src="@/assets/cup.png" id="cup"></div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'SecondRoundField',
  props: ['question', 'teams'],
  data(){
    return {
        count_question: 5
    }
  },
  methods: {
    calc_margin_hor: function () {
        if(this.current_template.numFields == 16)
            return "margin-left:5.6%";
        else if(this.current_template.numFields == 20)
            return "margin-left:3%";
        else if(this.current_template.numFields == 24)
            return "margin-left:1.2%";
    },
  },
  mounted: function () {
  this.$nextTick(function () {
    let ar = [];
    let start_left = '';
    let finish_left = '';
    let middle_left = '';
    let body = document.body;
    let body_bottom = body.getBoundingClientRect().bottom;
    let body_right = body.getBoundingClientRect().right;
    for( let j = 0; j < this.teams; ++j )
    {
      for( let i = 0; i < 5; ++i )
      {
        var cur_field = document.getElementById("second_round_question_" + String(j) + String(i));

        if( i == 2 )
        {
          let top = cur_field.getBoundingClientRect().top;
          let left = cur_field.getBoundingClientRect().left;
          middle_left = String(left/body_right * 100);
          ar.push(String(top/body_bottom * 100));
          continue;
        }
        cur_field.style.visibility = 'hidden';
      }
    }
    let start = document.getElementById("start_sec_round");
    let left = start.getBoundingClientRect().left;
    start_left = String(left/body_right * 100);

    let finish = document.getElementById("finish");
    left = finish.getBoundingClientRect().left;
    finish_left = String(left/body_right * 100);

    this.$emit('second-round-pos', ar, start_left, middle_left, finish_left);
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

#start_sec_round{
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
