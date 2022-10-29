<template>
     <div id="main_area">
        <div v-if="draw === 'questions'" class="grid" id="grid">
            <div class="scroll">
                <div class="grid_element" v-for="(option, index) in get_questions()" @click="choose_question" :key="index" :fake_id="option.tmp_id" v-bind:el_id="option.id">{{ option.shortText }}</div>
                <div class="grid_element plus" @click="add_question">+</div>
                <div class="grid_element load" id="load" @click="load_questions">Загрузить из файла</div>
            </div>
        </div>
        <div id="edit_window" class="edit_window" v-if="draw === 'edit'">
            <img @click="to_questions" src="@/assets/go_back.png" alt="">
            <input id="edit_window_input" class="edit_window_input" placeholder="Введите новое название продукта" :value="cache_product[0]" @blur="save_new_name" />
            <input id="input_path" placeholder="Введите название файла" />
            <div class="edit_window_color_title">Выберите новый цвет поля</div>
            <input id="color_switcher" type="color" style="width:35%;height:25%;" @input="save_new_color">
            <div class="edit_window_group_button">
                <div @click="save_edit" class="button">Сохранить</div>
                <div @click="reset" class="button">Сбросить</div>
            </div>
        </div>
        <div v-if="draw === 'delete'" class="delete_product_window" id="delete_product_window">
            <div id="delete_product_warning">
                Вы действительно хотите удалить поле "{{selected_product[0]}}" и все связанные с ним задания?
            </div>
            <div class="delete_window_group_button">
                <div @click="delete_product" class="button">Да</div>
                <div @click="cancel_product" class="button">Нет</div>
            </div>
        </div>
        <div v-if="draw === 'edit_question'" class="edit_question" id="edit_question">
            <div id="first_line" style="width:100%;">
                <img style="height:75%;width:7.5%;float:left;margin-right:5%" @click="to_questions" src="@/assets/go_back.png" alt="">
                <div style="width:50%;float:left;" id="short_name_header" resize="false">Краткое обозначение вопроса</div>
                <input style="float:left;width:30%;" type="text" maxlength="16" id="short_name" @blur="save_edit_changes" :value="selected_el.shortText" >
            </div>
            <div id="second_line" style="width:100%; height:15%;">
                <div style="width:30%;float:left;" id="category_header">Категория вопроса</div>
                <select style="float:left;width:30%;" id="type_selector" @blur="save_edit_changes" @input="update_is_media">
                    <option>С выбором ответа</option>
                    <option>Без выбора ответа</option>
                    <option>Вопрос-аукцион</option>
                    <option>Вопрос с медиа фрагментом</option>
                </select>
            </div>
            <div id="fiveth_line">
                <div id="wording_header" resize="false">Формулировка вопроса</div>
            </div>
            <textarea rows="12" id="wording" :value="selected_el.text" @blur="save_edit_changes"></textarea>
            <div id="fiveth_line">
                <div id="wording_header" resize="false">Ответ</div>
            </div>
            <textarea rows="2" id="answer" :value="selected_el.answer" @blur="save_edit_changes"></textarea>
            <input v-if="is_media" id="input_path" style="margin-left:12.5%;margin-top:2%;width:70%;" placeholder="Введите название файла" />
            <div class="delete_window_group_button">
                <div @click="save_question" class="button">Сохранить</div>
                <div @click="del_question" class="button">Удалить</div>
            </div>
            
        </div>
        <div v-if="draw === 'delete_question'" class="delete_product_window" id="delete_product_window">
            <div id="delete_product_warning">
                Вы действительно хотите удалить {{ selected_el.text }}?
            </div>
            <div class="delete_window_group_button">
                <div @click="delete_question" class="button">Да</div>
                <div @click="cancel_question" class="button">Нет</div>
            </div>
        </div>
        <div id="pop_up" class="pop_up" v-if="load">
            <div class="pop_up_title">Загрузка заданий</div>
            <input type="file" id="file-uploader" >
            <div class="pop_up_group_button">
                <div class="pop_up_button">Загрузить</div>
                <div @click="cancel_load" class="pop_up_button">Отмена</div>
            </div>
        </div>
    </div>
</template>

<script>
import { SERVER_PATH } from '../common_const.js'

let wording = '';
let type = '';

export default {
  name: 'QuestionsList',
  props:['selected_product', 'products', 'cache_product'],
  data(){
    return {
            selected: '1',
            selected_el: null,
            draw: 'questions',
            is_media: false
    }
  },
  methods: {
        reset: function () {
            this.$emit('reset-edit');
        },
        save_new_name: function () {
            var color = String(document.getElementById('color_switcher').value);
            var name = document.getElementById("edit_window_input").value;
            this.$emit('final-edit-product', name, 'color:white;background:' + color);
        },
        save_new_color: function () {
            var color = String(document.getElementById('color_switcher').value);
            this.$emit('final-edit-product', this.cache_product[0], 'color:white;background:' + color);
        },
        update_is_media: function () {
            if(document.getElementById('type_selector'))
                this.is_media = document.getElementById('type_selector').value == 'Вопрос с медиа фрагментом';
        },
        add_product: function () {
            this.$emit('add-field');
        },
        cancel_product: function () {
            var edit = document.getElementById("edit");
            var click_delete = document.getElementById("delete");
            click_delete.style.opacity = 1;
            edit.style.opacity = 1;
            this.draw = 'questions';
        },
        delete_product: function () {
            this.draw = 'questions';
            this.$emit('final-delete-product');
        },
        to_questions: function () {
            this.$emit('reset-edit');
            this.draw = 'questions';
            var edit = document.getElementById("edit");
            var click_delete = document.getElementById("delete");
            click_delete.style.opacity = 1;
            edit.style.opacity = 1;
        },
        save_edit: function () {
            this.$emit('save-edit');
            var edit = document.getElementById("edit");
            var click_delete = document.getElementById("delete");
            click_delete.style.opacity = 1;
            edit.style.opacity = 1;
        },
        choose_question: function (event) {
            this.draw = 'edit_question';
            this.selected = event.target.getAttribute("el_id");
            let fake_id = event.target.getAttribute("fake_id");
            var cur_el;
            var local_selected = this.selected;
            this.get_questions().forEach(function(item) {
                if(local_selected && local_selected == item.id)
                    cur_el = item;
                else if(fake_id && item.tmp_id == fake_id)
                    cur_el = item;
            });
            this.selected_el = cur_el;
            let mapping = new Map();
            mapping.set("TEXT", "Без выбора ответа").set("AUCTION", "Вопрос-аукцион").set("TEXT_WITH_ANSWERS", "С выбором ответа").set("MEDIA", "Вопрос с медиа фрагментом");
            wording = cur_el.text;
            
            type = mapping.get(cur_el.questionType);
            
        },
        del_question: function () {
            this.draw = 'delete_question';
        },
        delete_question: function () {
            this.draw = 'questions';
            var product_name = this.selected_product[0];
            var deleted_question = this.selected;
            this.products.forEach(function(item) {
                if(product_name == item.text)
                    item.questions = item.questions.filter(option => option.id != deleted_question);
            });
            fetch(SERVER_PATH + "/product/question/" + String(deleted_question), {
                method: "DELETE",
                headers: {'Content-Type': 'application/json'}
                });
        },
        cancel_question: function () {
            this.draw = 'edit_question';
        },
        save_question: function () {
            let mapping = new Map();
            mapping.set("Без выбора ответа", "TEXT").set("Вопрос-аукцион", "AUCTION").set("С выбором ответа", "TEXT_WITH_ANSWERS").set("Вопрос с медиа фрагментом", "MEDIA");
            this.$emit('edit-question', this.selected_product[0], this.selected_el, this.selected, mapping.get(document.getElementById("type_selector").value), document.getElementById("wording").value, document.getElementById('short_name').value, document.getElementById('answer').value);
            this.draw = 'questions';
        },
        get_questions: function () {
            var product_name = this.selected_product[0];
            var questions;
            this.products.forEach(function(item) {
                if(product_name == item.text)
                    questions = item.questions;
            });
            return questions;
        },
        add_question: function () {
            this.$emit('add-question', this.selected_product[0]);
        },
        check_delete_product: function () {
            this.draw = 'delete';
        },
        edit_product: function () {
            this.draw = 'edit';
        },
        save_edit_changes: function () {
            var product_name = this.selected_product[0];
            var selected = this.selected_el;
            this.products.forEach(function(item) {
                if(product_name == item.text)
                    item.questions.forEach(function(inner_item) {
                        if(selected == inner_item)
                        {
                            inner_item.shortText = document.getElementById('short_name').value;
                            inner_item.questionType = document.getElementById('type_selector').value;
                            inner_item.text = document.getElementById('wording').value;
                            inner_item.answer = document.getElementById('answer').value;
                        }
                    });
            });
        },
  },
}

document.addEventListener("DOMNodeInserted", function () {
    if(document.getElementById("type_selector")){
        var selector = document.getElementById("type_selector");
        selector.value = type;
        var wording_1 = document.getElementById("wording");
        wording_1.value = wording;
    }
    if(document.getElementById('color_switcher'))
    {
        let rgb2hex=c=>'#'+c.match(/\d+/g).map(x=>(+x).toString(16).padStart(2,0)).join``;
        document.getElementById('color_switcher').value = rgb2hex(document.getElementById('avatar').style.backgroundColor);
    }
}, false);
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
#main_area{
    float: left;
    height: 81%;
    width: 70%;
    margin-top: 3%;
    margin-left: 2%;
}

.grid{
    float: left;
    width: 100%;
    height: 100%;
}

.grid_element{
    margin-top: 1.1%;
    margin-left: 2%;
    margin-right: 2%;
    margin-bottom: 5%;
    float: left;
    width: 20%;
    height: 20%;
    border: 2px solid black;
    border-radius: 20px;
    text-align: center;
    font-size: 1.5vw;
    line-height: 7vw;
    color: black;
    transition: transform .25s ease;
}

.grid_element:hover {
  transform: scale(1.1); /* (150% zoom - Note: if the zoom is too large, it will go outside of the viewport) */
}

.plus, .load{
    border: 2px solid silver;
    color: silver;
}

.plus{
    font-size: 6vw;
}

.load{
    font-size: 1.3vw;
}

.scroll{
    height: 100%;
    overflow: auto;
    -ms-overflow-style: none;
    scrollbar-width: none;
}

.scroll::-webkit-scrollbar {
    width: 0;
    height: 0;
}

/* Pop up window */

.edit_window{
    float: left;
    position: fixed;
    width: 70%;
    height: 100%;
    background-color: white;
    text-align: center;
}

img{
    float: left;
    width: 7%;
    height: 7%;
}

.load_path{
    float: right;
    margin-top: 5%;
    margin-right: 8%;
    width: 12%;
    height: 12%;
}

.edit_window_color_title{
    margin-top: 4%;
    color: green;
    font-size: 1.6vw;
    width: 90%;
    margin-left: 5%;
    text-align: center;
    font-weight: bold;
    margin-bottom: 3%;
}

input{
    width: 60%;
    border-bottom: 1.5px solid silver;
    border-top: none;
    border-left: none;
    border-right: none;
    outline:none;
    font-size: 2vw;
}

#edit_window_input{
    margin-top: 5%;
    width: 80%;
}

#input_path{
    margin-top: 3%;
    margin-left: 7%;
    width: 80%;
}

.edit_window_group_button{
    margin-top: 2%;
    width: 90%;
    height: 20%;
    margin-left: 12%;
}

.edit_window_button{
    float: left;
    margin-left: 5%;
    margin-right: 5%;
    padding-top: 1%;
    padding-bottom: 1%;
    width: 30%;
    background-color: green;
    color: #ffffff;
    border-radius: 35px;
    font-size: 1.5vw;
}

.edit_window_button:hover {
    box-shadow: 0 0 10px 100px orange inset;
}

@import url(https://fonts.googleapis.com/css?family=Roboto:700);

#delete_product_window{
    margin-top: 13%;
    width: 100%;
}

#delete_product_warning{
    width: 80%;
    font-size: 250%;
    text-align: center;
}

.delete_window_group_button{
    width: 80%;
    margin-left: 15%;
}

.button{
    float: left;
    width: 30%;
    background-color: green;
    text-align: center;
    padding-top: 1.5%;
    padding-bottom: 1.5%;
    color: white;
    font-size: 1.2vw;
    font-weight: bold;
    border-radius: 20px;
    margin-left: 4%;
    margin-right: 10%;
    margin-top: 5%;
}

.button:hover{
    box-shadow: 0 0 10px 100px orange inset;
}

#edit_question{
    height: 40%;
}

#type_selector{
    width: 40%;
    font-size: 1.3vw;
    padding-top: 0.5%;
    padding-bottom: 0.5%;
    margin-left: 20%;
    margin-bottom: 2%;
}

#title{
    float: left;
    width: 60%;
    font-size: 2vw;
}

#wording{
    width: 70%;
    height: 75%;
    font-size: 1.4vw;
    resize: none;
    margin-left: 12.5%;
    margin-top: 1%;
}


/* Pop up window */

.pop_up{
    float: left;
    position: fixed;
    width: 45%;
    height: 35%;
    background-color: white;
    text-align: center;
    border-radius: 20px;
    border: 2px solid black;
    left: 27.5%;
    top: 32.5%;
}

.pop_up_title{
    margin-top: 2%;
    color: green;
    font-size: 2vw;
    font-weight: bold;
}

.pop_up_color_title{
    margin-top: 4%;
    color: green;
    font-size: 1.5vw;
    width: 90%;
    margin-left: 5%;
    text-align: center;
    font-weight: bold;
    margin-bottom: 3%;
}

input{
    width: 60%;
    border-bottom: 1.5px solid silver;
    border-top: none;
    border-left: none;
    border-right: none;
    outline:none;
    font-size: 1.3vw;
}

#pop_up_input{
    margin-top: 5%;
    margin-left: 5%;
    width: 70%;
}

.pop_up_group_button{
    margin-top: 5%;
    width: 90%;
    height: 20%;
    margin-left: 5%;
}

.pop_up_button{
    float: left;
    margin-left: 5%;
    margin-right: 5%;
    padding-top: 2.5%;
    padding-bottom: 2.5%;
    width: 40%;
    background-color: green;
    color: #ffffff;
    border-radius: 35px;
    font-size: 1.2vw;
}

.pop_up_button:hover {
    box-shadow: 0 0 10px 100px orange inset;
}


#file-uploader{
    margin-top: 5%;
}

#short_name{
    width: 40%;
}

#category_header{
    font-size: 1.65vw;
    margin-left: 12.5%;
}

#first_line{
    height: 20%;
}

#short_name_header{
    font-size: 1.65vw;
}

#wording_header{
    font-size: 1.65vw;
    margin-left: 12.5%;
    margin-top: 1%;
}

img{
    transition: transform .25s ease;
}

img:hover {
  transform: scale(1.1); /* (150% zoom - Note: if the zoom is too large, it will go outside of the viewport) */
}

#answer{
    width: 70%;
    height: 30%;
    font-size: 1.4vw;
    resize: none;
    margin-left: 12.5%;
    margin-top: 1%;
}
</style>
