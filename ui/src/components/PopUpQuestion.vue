<template>
    <div id="collapser">
       <div id="text">{{ collapse ? question[2] : question[1]}}</div>
       <img id="dots_pop_up" @click="pop_up" src="@/assets/dots.png" alt="">
       <img v-if="!collapse" src="@/assets/start.png" alt="" id="start_question" @click="start_timer">
       <div v-if="!collapse" id="answer_timer">00:30</div>
       <div id="question_area" v-if="!collapse">
           <div id="question_area_text">{{ question[0] }}</div> 
        </div>
       <div v-if="!collapse && tour == 1" id="header1">Зачесть пазл команде</div>
       <div v-if="!collapse && tour == 2" id="group_header">
           <div id="header2">Зачесть баллы команде</div>
       </div>
       <div v-if="!collapse" id="group_buttons">
           <div :style="'opacity:'+current_opacity[0]" class="team_button" @click="give_puzzle('team_1')">Команда 1</div>
           <div :style="'opacity:'+current_opacity[1]" class="team_button" @click="give_puzzle('team_2')">Команда 2</div>
           <div :style="'opacity:'+current_opacity[3]" class="team_button" @click="give_puzzle('team_3')">Команда 4</div>
           <div :style="'opacity:'+current_opacity[2]" class="team_button" @click="give_puzzle('team_4')">Команда 3</div>
       </div>
    </div>
</template>

<script>
export default {
  name: 'PopUpQuestion',
  props:['question', 'tour', 'turn', 'second_turn'],
  data(){
    return {
        collapse:true,
        current_opacity: [1,1,1,1]
    }
  },
  methods: {
    pop_up () {
        if(this.collapse)
        {
            document.getElementById('collapser').style.left = '22.5%'; 
            document.getElementById('collapser').style.top = '2%'; 
            document.getElementById('collapser').style.width = '55%';  
            document.getElementById('collapser').style.height = '95%';
            document.getElementById('text').style.marginTop = '2%';
            document.getElementById('text').style.color = 'green';
            document.getElementById('dots_pop_up').style.height = '5%';
            document.getElementById('dots_pop_up').style.marginTop = '1%';
            this.collapse = false;
        }
        else{
            document.getElementById('collapser').style.left = '30%'; 
            document.getElementById('collapser').style.top = '90%'; 
            document.getElementById('collapser').style.width = '40%';  
            document.getElementById('collapser').style.height = '5%';
            document.getElementById('text').style.marginTop = '0.5%';
            document.getElementById('text').style.color = 'black';
            document.getElementById('dots_pop_up').style.height = '90%';
            document.getElementById('dots_pop_up').style.marginTop = '0.25%';
            this.collapse = true;
        }
    },
    give_puzzle(team) {
        if(this.tour == 1)
        {
            document.getElementById('answer_timer').innerText = '00:30';
            this.$emit('give-puzzle', team, this.question[2]);
        }
        else
            this.$emit('give-score', team);
        
    },
    start_timer() {
        let field = document.getElementById('answer_timer');
        if(field.innerText != '00:30')
            return;
        var secs = 30;
        let timer = setInterval(tick,1000)
        function tick(){
            (--secs);
            if(secs < 10)
                field.innerText = '00:0' + secs;
            else
                field.innerText = '00:' + secs;
            if(secs == 0)
                clearInterval(timer);
}
    }
  },
  watch: {
        turn: function () {
            if(this.tour == 1 && this.turn == 0 || this.tour == 2 && (this.turn == 0 || this.turn == 4))
            {
                this.current_opacity[0] = 1;
                this.current_opacity[1] = 0.5;
                this.current_opacity[2] = 0.5;
                this.current_opacity[3] = 0.5;
            }
            else if(this.tour == 1 && this.turn == 1 || this.tour == 2 && (this.turn == 1 || this.turn == 5))
            {
                this.current_opacity[0] = 0.5;
                this.current_opacity[1] = 1;
                this.current_opacity[2] = 0.5;
                this.current_opacity[3] = 0.5;
            }
            else if(this.tour == 1 && this.turn == 2 || this.tour == 2 && (this.turn == 2 || this.turn == 6))
            {
                this.current_opacity[0] = 0.5;
                this.current_opacity[1] = 0.5;
                this.current_opacity[2] = 1;
                this.current_opacity[3] = 0.5;
            }
            else if(this.tour == 1 && this.turn == 3 || this.tour == 2 && (this.turn == 3 || this.turn == 7))
            {
                this.current_opacity[0] = 0.5;
                this.current_opacity[1] = 0.5;
                this.current_opacity[2] = 0.5;
                this.current_opacity[3] = 1;
            }
        },
        second_turn: function () {
            if(this.second_turn == 1 || this.second_turn == 5)
            {
                this.current_opacity[0] = 1;
                this.current_opacity[1] = 0.5;
                this.current_opacity[2] = 0.5;
                this.current_opacity[3] = 0.5;
            }
            else if(this.second_turn == 2 || this.second_turn == 6)
            {
                this.current_opacity[0] = 0.5;
                this.current_opacity[1] = 1;
                this.current_opacity[2] = 0.5;
                this.current_opacity[3] = 0.5;
            }
            else if(this.second_turn == 3 || this.second_turn == 7)
            {
                this.current_opacity[0] = 0.5;
                this.current_opacity[1] = 0.5;
                this.current_opacity[2] = 1;
                this.current_opacity[3] = 0.5;
            }
            else if(this.second_turn == 4 || this.second_turn == 8)
            {
                this.current_opacity[0] = 0.5;
                this.current_opacity[1] = 0.5;
                this.current_opacity[2] = 0.5;
                this.current_opacity[3] = 1;
            }
        },
    }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
#collapser{
    height: 5%;
    width: 40%;
    position: absolute;
    left: 30%;
    top: 90%;
    border: solid black 2px;
    background-color: white;
    border-radius: 5px;
    z-index: 5;
}

#text{
    width: 90%;
    float: left;
    margin-top: 0.5%;
    margin-left: 5%;
    text-align: center;
    font-size: 1.5vw;
}

#dots_pop_up{
    width: 5%;
    height: 90%;
    float: right;
    margin-top: 0.25%;
}

#question_area{
    margin-top: 12%;
    margin-left: 10%;
    width: 80%;
    height: 65%;
    border: solid 2px;
    border-radius: 3%;
}

#question_area_text{
    margin-top: 5%;
    margin-left: 5%;
    width: 90%;
    height: 90%;
    text-align: justify;
    text-indent: 5%;
    font-size: 1.6vw;
}

#start_question{
    width: 5%;
    height: 5%;
    margin-top: 1%;
    margin-right: 1%;
    margin-left: 42%;
    float: left;
}

img{
  transition: transform .25s ease;
}

img:hover {
  transform: scale(1.1); /* (150% zoom - Note: if the zoom is too large, it will go outside of the viewport) */
}

#answer_timer{
    width: 10%;
    height: 3%;
    float: left;
    font-size: 2vw;
    margin-top: 1%;
}

#group_buttons{
    width: 80%;
    height: 10%;
    margin-left: 10%;
}

.team_button{
    margin-left: 10%;
    margin-right: 10%;
    margin-top: 1%;
    height: 20%;
    width: 25%;
    float: left;
    background: green;
    border-radius: 10px;
    padding: 1.5% 2% 1.5% 2%;
    text-align: center;
    color: white;
    font-size: 1vw;
}

.team_button:hover {
    box-shadow: 0 0 10px 100px orange inset;
}

#header1{
    margin-top: 1.5%;
    margin-bottom: 0.5%;
    font-size: 1.5vw;
    text-align: center;
}

#group_header{
    width: 100%;
    float: left;
}

#header2{
    margin-top: 1.5%;
    margin-bottom: 0.5%;
    font-size: 1.5vw;
    text-align: center;
    float: left;
    margin-left: 25%;
    width: 50%;
}

#header3{
    margin-top: 1.5%;
    margin-bottom: 0.5%;
    font-size: 150%;
    text-align: center;
    width: 25%;
    float: left;
}

input{
    margin-top: 2%;
    margin-bottom: 0.5%;
    width:5%;
    float: left;
}
</style>
