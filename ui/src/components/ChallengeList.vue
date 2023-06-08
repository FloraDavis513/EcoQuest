<template>
    <div v-if="!selected_challenge" class="scroll">
     <div class="grid" id="grid">
        <div v-for="(option, index) in challenges" :key="index" class="cell">
            <div class="grid_element"  @click="select_challenge(option.challengeId)"  v-bind:style="option.colour">
                {{ option.name }}
            </div>
        </div>
        <div class="cell">
            <div class="grid_element plus" @click="add_challenge">+</div>
        </div>
     </div>
    </div>
    <div v-else class="challenge_grid">
        <div class="menu_grid">
            <input id="challenge_name" type="text" placeholder="Соревнование" :value="selected_challenge.name" @blur="save_tempalte_name">
            <input id="challenge_password" type="text" placeholder="Пароль" :value="selected_challenge.password" @blur="save_tempalte_name">
            <div id="first_group_button">
                <div class="button" id="save" @click="save_challenge">
                    Сохранить
                </div>
                <div class="button" id="delete" @click="delete_challenge">
                    Удалить
                </div>
                <div class="button" @click="back_to_challenges" >
                    Назад
                </div>
            </div>
        </div>
        <div class="scroll" style="width: 77.5%;">
        <div v-for="(product,index) in products" :key="index">
            <div class="product_row">
                <div class="product_name" style="width: 95%;">
                    {{product.name}}
                </div>
                <div class="expand_button" @click="expand_product(product, $event)"><img class="polygon" src="@/assets/Polygon_1.png"></div>
            </div>
            <div class="table_headers product_row" v-show="product.is_visible">
                <div class="last_redaction" style="font-weight:bold;text-decoration:underline;text-align:center;margin-left:3%;">
                Изменен
                </div>
                <div class="name_quest2" style="font-weight: bold;text-decoration:underline;margin-left: 12%;">
                Краткое название
                </div>
                <div class="type_quest" style="font-weight: bold;text-decoration: underline;margin-left: 15%;">
                Тип
                </div>
                <div class="check_quest" :style="'margin-left:39.5%;'">
                    <!-- <input :id="'product_check_' + index" class="check_button" type="checkbox" :checked="product.activeQuestions.length != 0" @click="change_all_status(product.productId)"> -->
                    <!-- <input :id="'product_check_' + index" class="check_button" type="checkbox"> -->
                </div>
            </div>
            <div v-for="(question,question_index) in product.questions" :key="question_index" class="table_headers product_row" v-show="product.is_visible" >
                <div style="margin-left:3%;">
                {{beautify_date(question.lastEditDate)}}
                </div>
                <div style="margin-left:10%;width:300px;">
                {{question.shortText}}
                </div>
                <div style="margin-left:10%;width:250px;">
                {{get_readiable_type(question.type)}}
                </div>
                <div class="check_quest" style="margin-left:25%;">
                    <input class="check_button" type="checkbox" :checked="selected_questions && selected_questions.has(question.questionId)" @click="select_question(question.questionId)">
                </div>
            </div>
        </div>
        </div>
    </div>
    
</template>

<script>
import { SERVER_PATH } from '../common_const.js'
import { parse } from 'date-fns';

export default {
  name: 'ChallengeList',
  data(){
    return {
        challenges: [],
        products: [],
        selected_challenge: null,
        selected_questions: null
    }
  },
  methods: {
        add_challenge: async function () {
            await fetch(SERVER_PATH + "/challenge/update", {
            method: "POST",
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify({name: "Соревнование", password: "", questions: JSON.stringify([])})
            });
            this.update_challenges();
        },
        delete_challenge: async function () {
            await fetch(SERVER_PATH + "/challenge/delete/" + this.selected_challenge.challengeId, {
            method: "DELETE",
            headers: {'Content-Type': 'application/json'},
            });
            this.update_challenges();
            this.back_to_challenges();
        },
        select_challenge: async function (id) {
            await fetch(SERVER_PATH + "/challenge/get/" + id, {
            method: "GET",
            headers: {'Content-Type': 'application/json'}
            }).then( res => res.json() ).then( data => this.selected_challenge = data);
            const array = JSON.parse("[" + this.selected_challenge.questions + "]");
            this.selected_questions = new Set(array);

        },
        back_to_challenges: function () {
            this.selected_challenge = null;
            this.selected_questions = null;
            this.close_all();
        },
        update_challenges: function () {
            fetch(SERVER_PATH + "/challenge/get/all", {
            method: "GET",
            headers: {'Content-Type': 'application/json'}
            }).then( res => res.json() ).then( data => this.challenges = data );
        },
        close_all: function () {
            this.products.forEach(product => product.is_visible = false)
        },
        expand_product: function (product, event) {
            product.is_visible = !product.is_visible;    
            let finded_target = event.target.className == 'polygon' ? event.target : event.target.firstElementChild;
            if(finded_target.style.transform == 'rotate(180deg)')
                finded_target.style = 'transform:rotate(0deg);';
            else
                finded_target.style = 'transform:rotate(180deg);';
        },
        beautify_date: function (date) {
            let a = parse(date, 'M/d/yyyy hh:mm:ss aa', new Date());
            const full_date = a.toLocaleString('ru-RU');
            return full_date.slice(0, 11);
        },
        get_readiable_type: function (type) {
            let mapping = new Map();
            mapping.set("TEXT", "Без выбора ответа").set("AUCTION", "Вопрос-аукцион").set("TEXT_WITH_ANSWERS", "С выбором ответа").set("MEDIA", "Вопрос с медиа фрагментом");
            return mapping.get(type);
        },
        select_question: function (id) {
            if(this.selected_questions.has(id))
                this.selected_questions.delete(id);
            else
                this.selected_questions.add(id);
        },
        save_challenge: async function () {
            let questions_str = "";
            this.selected_questions.forEach(question => questions_str += question + ',');
            await fetch(SERVER_PATH + "/challenge/update", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({challengeId: this.selected_challenge.challengeId, name: document.getElementById("challenge_name").value,
                                      password: document.getElementById("challenge_password").value, questions: questions_str.slice(0, -1)})
            });
            this.update_challenges();
            this.back_to_challenges();
        },
  },
  beforeMount: function () {
    this.update_challenges();
    fetch(SERVER_PATH + "/product/get/all/3", {
            method: "GET",
            headers: {'Content-Type': 'application/json'}
            }).then( res => res.json() ).then( data => this.products = data).then(this.close_all());
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.grid{
    margin-top: 3%;
    margin-left: 5%;
    width: 90%;
    height: 80%;
}

.grid_element{
    margin-bottom: 5%;
    float: left;
    width: 80%;
    height: 100%;
    border: 0.15vw solid black;
    border-radius: 1vw;
    text-align: center;
    font-size: 220%;
    font-size: 1.8vw;
    line-height: 100px;
    line-height: 6.3vw;
    transition: transform .25s ease;
}

.grid_element:hover {
  transform: scale(1.1); /* (150% zoom - Note: if the zoom is too large, it will go outside of the viewport) */
}

.number{
    color: black;
    font-size: 2vw;
}

.plus{
    font-size: 6vw;
    border: 2px solid silver;
    color: silver;
}

.scroll{
    height: 86%;
    overflow: auto;
    -ms-overflow-style: none;
    scrollbar-width: none;
    margin-top: 1%;
    width: 100%;
}

.scroll::-webkit-scrollbar {
    width: 0;
    height: 0;
}

.cell{
    float: left;
    width: 20%;
    margin-bottom: 2.5%;
    display: flex;
    flex-direction: column;
    justify-content: space-around;
    align-items: center;
    text-align: center;
}

.challenge_grid{
    width: 100%;
    float: left;
    display: flex;
    flex-direction: row;
    height: 87.5%;
    font-size: 1.3vmax;
}

.menu_grid{
    margin-top: 0.5vmax;
    margin-left: 2.5vmax;
    margin-right: 1vmax;
    padding-right: 2.5vmax;
    border-right: solid silver 2px;
    display: flex;
    flex-direction: column;
    align-items: center;
}

#challenge_name{
  margin-top: 2vmax;
  text-align: center;
  font-size: 1.3vw;
  border-radius: 0.5vmax;
  height: 6vmax;
  margin-bottom: 2vmax;
}

#challenge_password{
    font-size: 1.3vw;
    margin-bottom: 2vmax;
}

.button{
    width: 15vmax;
    margin-top: 1vmax;
    background-color: green;
    color: #ffffff;
    font-weight: bold;
    border-radius: 1vmax;
    padding-top: 0.75vmax;
    padding-bottom: 0.75vmax;
    text-align: center;
}

.button:hover {
    box-shadow: 0 0 10px 100px orange inset;
}

.product_row{
    display: flex;
    flex-direction: row;
    border: solid black 2px;
    border-radius: 0.5vmax;
    padding: 0.5vmax;
    margin-top: 0.5vmax;
    background-color: silver;
}

.product_name{
    font-size: 1.5vmax;
    text-align: center;
}

.table_headers{
    background-color: white;
    margin-top: 0;
}

.check_button{
    height: 1.5vmax;
    width: 1.5vmax;
}

</style>
