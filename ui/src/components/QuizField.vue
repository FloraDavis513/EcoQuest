<template>
    <div id="field" v-if="quiz.questions.length > 0">
        <div id="question_counter" class="margin_field">
            {{"Вопрос " + Number(current_question + 1) + " из " + quiz.questions.length}}
        </div>
        <div v-show="false" id="time_counter" class="margin_field">
            {{"Прошедшее время: " + beautify_timer()}}
        </div>
        <div class="helps_group">
            <div style="margin-right:1vmax;">Подсказки:</div>
            <div v-show="quiz.helps.Fifty" class="helps_group">
                <div id="fifty" class="help_img" title="Убрать два неверных ответа" @click="use_fifty">50:50</div>
                <div class="help_counter">{{'&#215; ' + quiz.helps.Fifty}}</div>
            </div>
            <div v-show="quiz.helps.RightMistake" class="helps_group">
                <div id="right_mistake" class="help_img" title="Право на ошибку" @click="use_right_mistake">2 шанс</div>
                <div class="help_counter">{{'&#215; ' + quiz.helps.RightMistake}}</div>
            </div>
        </div>
        <div id="question_field" class="margin_field">
            <div id="question_header" class="margin_field">
                {{quiz.questions[current_question].productName}}
            </div>
            <div id="question_body" class="margin_field scroll">
                <img v-if="quiz.questions[current_question].type == 'MEDIA' && quiz.questions[current_question].media" :src="get_media()" style="margin-bottom:1vmax;">
                <div>
                    {{quiz.questions[current_question].text}}
                </div>
            </div>
        </div>
        <div v-if="quiz.questions[current_question].type == 'TEXT_WITH_ANSWERS'" id="answer_group">
            <div class="row">
                <div class="answer" @click="enter_answer">
                    {{quiz.questions[current_question].answers.AllAnswers[0]}}
                </div>
                <div class="answer" @click="enter_answer">
                    {{quiz.questions[current_question].answers.AllAnswers[1]}}
                </div>
            </div>
            <div class="row">
                <div class="answer" @click="enter_answer">
                    {{quiz.questions[current_question].answers.AllAnswers[2]}}
                </div>
                <div class="answer" @click="enter_answer">
                    {{quiz.questions[current_question].answers.AllAnswers[3]}}
                </div>
            </div>
        </div>
            <div v-else-if="quiz.questions[current_question].type == 'TEXT'" id="answer_group" class="margin_field">
            <input id="text_answer" class="answer input_answer" type="text" placeholder="Введите ответ" v-on:keyup.enter="enter_text_answer">
            <div v-show="!answer_send" class="row">
                <div class="answer quiz_button" @click="clear_answer">
                    Очистить
                </div>
                <div class="answer quiz_button" @click="enter_text_answer">
                    Отправить
                </div>
            </div>
            <div v-show="show_correct_answer" id="correct_answer" class="answer input_answer margin_field"> 

            </div>
        </div>
        <div v-else-if="quiz.questions[current_question].type == 'MEDIA' && quiz.questions[current_question].media" id="answer_group" class="margin_field">
            <div v-if="quiz.questions[current_question].answers.AllAnswers.length == 1">
                <input id="text_answer" class="answer input_answer" type="text" placeholder="Введите ответ" v-on:keyup.enter="enter_text_answer">
                <div v-show="!answer_send" class="row">
                    <div class="answer quiz_button" @click="clear_answer">
                        Очистить
                    </div>
                    <div class="answer quiz_button" @click="enter_text_answer">
                        Отправить
                    </div>
                </div>
                <div v-show="show_correct_answer" id="correct_answer" class="answer input_answer margin_field"/>
            </div>
            <div v-else>
                <div class="row">
                    <div class="answer" @click="enter_answer">
                        {{quiz.questions[current_question].answers.AllAnswers[0]}}
                    </div>
                    <div class="answer" @click="enter_answer">
                        {{quiz.questions[current_question].answers.AllAnswers[1]}}
                    </div>
                </div>
                <div class="row">
                    <div class="answer" @click="enter_answer">
                        {{quiz.questions[current_question].answers.AllAnswers[2]}}
                    </div>
                    <div class="answer" @click="enter_answer">
                        {{quiz.questions[current_question].answers.AllAnswers[3]}}
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import {SERVER_PATH, SRC_PATH} from "@/common_const";

export default {
  name: 'QuizField',
  components: {
  }, 
  data(){
    return {
        quiz: {questions:[]},
        current_question: 0,
        elapsed_seconds: 0,
        answer_send: false,
        right_mistake: false
    }
  },
  props: ['quiz_products', 'mode'],
  methods:{
    end_quiz: function () {
        this.$emit('end-quiz');
    },
    reset_answer: function (timeout) {
        setTimeout(() => {
                    if(this.current_question == this.quiz.questions.length - 1)
                        this.end_quiz();
                    ++this.current_question;
                    Array.prototype.forEach.call(document.getElementsByClassName("answer"), function(elem) {
                        elem.style.backgroundColor = 'white';
                        elem.style.visibility = 'visible';
                    });
                    Array.prototype.forEach.call(document.getElementsByClassName("quiz_button"), function(elem) {
                        elem.style.backgroundColor = '#C4C4C4';
                    });
                    this.answer_send = false;
                    this.show_correct_answer = false;
                    this.right_mistake = false;
                    if(document.getElementById("text_answer"))
                        document.getElementById("text_answer").value = "";
                }, timeout);
    },
    new_try: function () {
        setTimeout(() => {
                    Array.prototype.forEach.call(document.getElementsByClassName("answer"), function(elem) {
                        elem.style.backgroundColor = 'white';
                        elem.style.visibility = 'visible';
                    });
                    Array.prototype.forEach.call(document.getElementsByClassName("quiz_button"), function(elem) {
                        elem.style.backgroundColor = '#C4C4C4';
                    });
                    this.answer_send = false;
                    this.show_correct_answer = false;
                    this.right_mistake = false;
                }, 1000);
    },
    enter_answer: function (event) {
        event.target.style.backgroundColor = 'grey';
        const start = performance.now();
        fetch(SERVER_PATH + '/quiz/checkAnswer', {
                method: 'POST',
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({Answer: event.target.innerText, QuestionId: this.quiz.questions[this.current_question].questionId,
                                      UserId: JSON.parse(localStorage.getItem('user')).userId, Duration: this.elapsed_seconds, RightMistake: this.right_mistake})
              }).then(res => res.json()).then(data => {
                const end = performance.now();
                if( end - start < 500 )
                    this.sleep(500 - (end - start));
                if(data.result)
                    event.target.style.backgroundColor = 'lime';
                else
                {
                    event.target.style.backgroundColor = 'red';
                    if(this.right_mistake)
                    {
                        this.right_mistake = false;
                        return;
                    }
                    Array.prototype.forEach.call(document.getElementsByClassName("answer"), function(elem) {
                        if(elem.innerText == data.correct_answer)
                            elem.style.backgroundColor = 'lime';
                    });
                }
                this.reset_answer(2000);
              });
    },
    enter_text_answer: function () {
        this.answer_send = true;
        const text_answer = document.getElementById("text_answer");
        text_answer.style.backgroundColor = 'grey';
        const start = performance.now();
        fetch(SERVER_PATH + '/quiz/checkAnswer', {
                method: 'POST',
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({Answer: text_answer.value, QuestionId: this.quiz.questions[this.current_question].questionId,
                                      UserId: JSON.parse(localStorage.getItem('user')).userId, Duration: this.elapsed_seconds, RightMistake: this.right_mistake})
              }).then(res => res.json()).then(data => {
                const end = performance.now();
                if( end - start < 500 )
                    this.sleep(500 - (end - start));
                if(data.result)
                    text_answer.style.backgroundColor = 'lime';
                else
                {
                    text_answer.style.backgroundColor = 'red';
                    if(this.right_mistake)
                    {
                        this.right_mistake = false;
                        this.new_try();
                        return;
                    }
                    this.show_correct_answer = true;
                    const correct_answer = document.getElementById("correct_answer");
                    correct_answer.style.backgroundColor = 'lime';
                    correct_answer.innerText = data.correct_answer;
                }
                this.reset_answer(3000);
              });
    },
    sleep: function (milliseconds) {
        var start = new Date().getTime();
        for (var i = 0; i < 1e7; i++) {
            if ((new Date().getTime() - start) > milliseconds){
            break;
            }
        }
    },
    second_left: function() {
        ++this.elapsed_seconds;
    },
    beautify_timer: function() {
        const elapsed_minutes = Math.floor(this.elapsed_seconds / 60);
        const remain_seconds = this.elapsed_seconds - elapsed_minutes * 60;
        return ((elapsed_minutes < 10) ? ("0" + String(elapsed_minutes)) : elapsed_minutes) + ":" + ((remain_seconds < 10) ? ("0" + String(remain_seconds)) : remain_seconds);
    },
    clear_answer: function() {
        document.getElementById("text_answer").value = "";
    },
    use_fifty: function() {
        if(this.quiz.helps.Fifty < 1 || this.quiz.questions[this.current_question].type != 'TEXT_WITH_ANSWERS')
            return;
        --this.quiz.helps.Fifty;
        let correct_answer = this.quiz.questions[this.current_question].answers.CorrectAnswers[0];
        let counter = 0;
        Array.prototype.forEach.call(document.getElementsByClassName("answer"), function(elem) {
                        if(counter < 2 && elem.innerText != correct_answer)
                        {
                            elem.style.visibility = 'hidden';
                            ++counter;
                        }
                    });
        fetch(SERVER_PATH + '/quiz/help', {
                method: 'POST',
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({Help: 1, QuestionId: this.quiz.questions[this.current_question].questionId,
                                      UserId: JSON.parse(localStorage.getItem('user')).userId, Duration: this.elapsed_seconds})
              })
    },
    use_right_mistake: function() {
        if(this.quiz.helps.RightMistake < 1)
            return;
        this.right_mistake = true;
        --this.quiz.helps.RightMistake;
        fetch(SERVER_PATH + '/quiz/help', {
                method: 'POST',
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({Help: 2, QuestionId: this.quiz.questions[this.current_question].questionId,
                                      UserId: JSON.parse(localStorage.getItem('user')).userId, Duration: this.elapsed_seconds})
              })
    },
    get_media: function () {
        const regex = /media\d+.\w+/g;
        const found = this.quiz.questions[this.current_question].media.match(regex);
        return SRC_PATH + found + '?' + Date.now();
    },
  },
  beforeCreate: async function() {
    if(this.mode == 'challenge')
    {
        await fetch(SERVER_PATH + '/quiz/get/challenge', {
                        method: 'POST',
                        headers: {'Content-Type': 'application/json'},
                        body: JSON.stringify({UserId: JSON.parse(localStorage.getItem('user')).userId, mode: this.mode, password: "Пароль"})
                    }).then(res => res.json()).then(data => this.quiz = data);
        this.quiz.questions = JSON.parse(this.quiz.questions);
        this.quiz.helps = JSON.parse(this.quiz.helps);
        this.quiz.questions.forEach(question => {
            question.answers = JSON.parse(question.answers);
            });
        return;
    }
    if(this.quiz_products.length == 0)
    {
        await fetch(SERVER_PATH + '/quiz/get/random/' + JSON.parse(localStorage.getItem('user')).userId, {
                        method: 'GET',
                        headers: {'Content-Type': 'application/json'},
                    }).then(res => res.json()).then(data => this.quiz = data);
        this.quiz.questions = JSON.parse(this.quiz.questions);
        this.quiz.helps = JSON.parse(this.quiz.helps);
        this.quiz.questions.forEach(question => {
            question.answers = JSON.parse(question.answers);
        });
    }
    else
    {
        await fetch(SERVER_PATH + '/quiz/get', {
                        method: 'POST',
                        headers: {'Content-Type': 'application/json'},
                        body: JSON.stringify({UserId: JSON.parse(localStorage.getItem('user')).userId, SelectedProduct: this.quiz_products, mode: this.mode})
                    }).then(res => res.json()).then(data => this.quiz = data);
        this.quiz.questions = JSON.parse(this.quiz.questions);
        this.quiz.helps = JSON.parse(this.quiz.helps);
        this.quiz.questions.forEach(question => {
            question.answers = JSON.parse(question.answers);
        });
    }
  },
  mounted: function() {
    setInterval(this.second_left, 1000);
  },
  updated: function() {
    if( document.getElementById("fifty") && this.quiz.questions[this.current_question].type != 'TEXT_WITH_ANSWERS' )
        document.getElementById("fifty").style.opacity = 0.5;
    else if( document.getElementById("fifty") && this.quiz.questions[this.current_question].type == 'TEXT_WITH_ANSWERS' )
        document.getElementById("fifty").style.opacity = 1;
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
#field{
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    text-align: center;
    font-size: 1.9vmax;
}

#question_field{
    border: solid 0.2vmax black;
    border-radius: 1vmax;
    width: 47.5vmax;
    max-width: 80%;
    height: 20vmax;
}

#answer_group{
    max-width: 80%;
    width: 47.5vmax;
    height: 12.5vmax;
    display: flex;
    /* justify-content: center; */
    flex-direction: column;
    align-items: center;
    text-align: center;
}

.answer{
    display: flex;
    justify-content: center;
    flex-direction: column;
    align-items: center;
    max-width: 42.5%;
    width: 20vmax;
    height: 5vmax;
    border: solid 0.2vmax black;
    border-radius: 1vmax;
}

@media screen and (max-width: 800px) {
  .answer {
    height: 10vmax;
  }
}

.input_answer{
    width: 47.5vmax;
    max-width: 100%;
}

input.input_answer{
    font-size: 1.9vmax;
    text-indent: 2vmax;
}

#text_answer{
    font-size: 1.9vmax;
}

.row{
    width: 47.5vmax;
    max-width: 100%;
    margin-top: 1vmax;
    display: flex;
    justify-content: space-between;
    align-items: center;
    text-align: center;
}

.margin_field{
    margin-top: 0.8vmax;
}

.scroll{
    height: 70%;
    width: 90%;
    margin-left: 5%;
    padding-right: 5%;
    overflow: auto;
    -ms-overflow-style: none;
}

.quiz_button{
    background-color: #C4C4C4;
    border: none;
}

.helps_group{
    display: flex;
    flex-direction: row;
    justify-content: center;
    align-items: center;
    text-align: center;
    font-size: 1.9vmax;
    margin-top: 0.5vmax;
}
.help_img{
    width: 5vmax;
    height: 2vmax;
    border: solid 0.25vmax black;
    border-radius: 1vmax;
    padding: 1vmax 0.5vmax 1vmax 0.5vmax;
    text-align: center;
}

.help_img:hover{
    border: solid 0.25vmax red;
}

.help_counter{
    margin-left: 0.5vmax;
    margin-right: 0.5vmax;
}

#right_mistake{
    font-size: 1.5vmax;
}

@media screen and (max-width: 800px) {
  #right_mistake {
    font-size: 1.45vmax;
  }
}
</style>