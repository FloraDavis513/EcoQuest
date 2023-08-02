<template>
     <div id="main_area">
        <div v-if="draw === 'questions'" class="grid" id="grid">
            <!-- <div class="scroll">
                <div class="grid_element" v-for="(option, index) in selected_product.questions" @click="choose_question(index)" :key="index">{{ option.shortText }}</div>
                <div class="grid_element plus" @click="add_question">+</div>
            </div> -->
            <div v-for="(option, index) in selected_product.questions" :key="index" class="cell">
                <div  class="weight" :style="'background-color: ' + option.weight"></div>
                <div class="grid_element" @click="choose_question(index)">
                    {{ option.shortText !== "" ? option.shortText : ( "Вопрос " + ( String(index + 1) ) ) }}
                </div>
            </div>
            <div class="cell">
                <div class="weight" style="background-color:white;"></div>
                <div class="grid_element plus" @click="add_question">+</div>
            </div>
        </div>
        <div id="edit_window" class="edit_window" v-if="draw === 'edit'">
            <img @click="to_questions" src="@/assets/go_back.png" alt="">
            <div :style="'width:15%;height:15%;margin-left:2.5%;float:left;position:relative;'" id="selected_logo_preview">
                <img id="logo_preview" :src="get_preview()" style="position:absolute;width:100%;height:100%;">
                <input id="selected_logo" type="file" name="uploads" style="width:100%;height:100%;" @change="upload_logo">
            </div>
            <!-- <div v-else style="width:15%;height:15%;margin-left:5%;float:left;border:0.15vw solid silver;border-radius:0.75vw;color:silver;font-size:2vw;text-align:center;position:relative;">
                <p style="position:absolute;left:30%;top:1%;">Лого</p> 
                <input id="selected_logo" type="file" name="uploads" style="width:100%;height:100%;" @change="upload_logo">
            </div> -->
            <div :style="'width:15%;height:15%;margin-left:2.5%;float:left;border-radius:0.75vw;' + new_version_product.color">
                <input id="color_switcher" type="color" style="width:100%;height:100%;opacity:0;" @input="save_new_color">
            </div>
            <input id="edit_window_input" class="edit_window_input" placeholder="Введите новое название продукта" :value="new_version_product.name" @blur="save_new_name" />
            <div>
                <div style="float:left;font-size:1.5vmax;margin-top:2.5%;text-align:left;margin-left:2%;">Связать с:</div>
                <select v-show="current_round == 3" id="relation" style="font-size:1.5vmax;margin-top:2.5%;text-align:left;margin-left:2%;" @input="change_relation">
                    <option value="Не выбрано">Не выбрано</option>
                    <option v-for="(product, index) in first_round_products" :key="index" :value="product.name">{{product.name}}</option>
                </select>
            </div>
            <div class="edit_window_group_button">
                <div @click="save_edit" style="margin-left:25%;" class="button">Сохранить</div>
                <div @click="reset" class="button" style="margin-left: 10%;">Сбросить</div>
            </div>
        </div>
        <div v-if="draw === 'delete'" class="delete_product_window" id="delete_product_window">
            <div id="delete_product_warning">
                Вы действительно хотите удалить поле "{{selected_product.name}}" и все связанные с ним задания?
            </div>
            <div class="delete_window_group_button">
                <div @click="delete_product" class="button">Да</div>
                <div @click="cancel_product" class="button">Нет</div>
            </div>
        </div>
        <div v-if="draw === 'edit_question'" id="edit_question" style="height:95%">
            <div id="first_line" style="width:100%;height:7.5%;">
                <img style="height:75%;width:7.5%;float:left;margin-right:5%" @click="to_questions" src="@/assets/go_back.png" alt="">
                <div style="width:50%;float:right;font-size:1.25vw;font-style:italic;" id="short_name_header" resize="false">{{'Дата последнего редактирования: ' + beautify_date(selected_question.lastEditDate)}}</div>
                <!-- <img style="height:75%;width:7.5%;float:left;margin-right:5%" @click="to_questions" src="@/assets/go_back.png" alt="">
                <div style="width:50%;float:left;" id="short_name_header" resize="false">Краткое обозначение вопроса</div>
                <input style="float:left;width:30%;" type="text" maxlength="16" id="short_name" @blur="save_edit_changes" :value="selected_question.shortText" > -->
            </div>
            <div class="scroll" style="height:80.5%;box-shadow: inset 0 0 20px 1px lightgrey;padding-bottom:2%;">
                <div v-if="current_round == 3" id="second_line" style="width:100%;height:10%;margin-top:2%;">
                    <div style="width:30%;float:left;" id="category_header">Связан с:</div>
                    <select style="float:left;width:30%;" id="question_relation" @input="change_relation_question">
                        <option>Не выбрано</option>
                        <option v-for="(question, index) in questions_to_relation" :key="index" :value="question.shortText">{{question.shortText}}</option>
                    </select>
                </div>
                <div id="second_line" style="width:100%;height:10%;margin-top:2%;">
                    <div style="width:30%;float:left;" id="category_header">Категория вопроса</div>
                    <select style="float:left;width:30%;" id="type_selector" :value="displayed_question_type" @input="update_is_media">
                        <option>С выбором ответа</option>
                        <option>Без выбора ответа</option>
                        <option>Вопрос-аукцион</option>
                        <option>Вопрос с медиа фрагментом</option>
                    </select>
                </div>
                <div id="second_line" style="width:100%; height:10%;margin-left:12.5%;">
                    <div style="width:50%;float:left;" id="short_name_header" resize="false">Краткое обозначение вопроса</div>
                    <input style="float:left;width:30%;" type="text" maxlength="16" id="short_name" @blur="save_edit_changes" :value="selected_question.shortText" >
                </div>
                <div id="fiveth_line" style="float:left;width:100%;">
                    <div id="wording_header" resize="false" style="float:left;margin-bottom:1%;">Формулировка вопроса</div>
                    <div v-if="is_media" @click="media_preview = true" class="button" style="float:right;width:21.35%;margin-top:1.5%;margin-right:10%;padding-top:1.07%;padding-bottom:1.07%;">Выбрать медиа</div>
                </div>
                <textarea rows="12" id="wording" :value="selected_question.text" @blur="save_edit_changes"></textarea>
                <div style="width:30%;height:50%;float:left;margin-left:16%;margin-top:3%;border:2px solid silver;border-radius:0.75vw;">
                    <div style="text-align:center;font-size: 1.65vw;">Верные ответы</div>
                    <hr>
                    <div class="scroll" style="height:70%;width:100%;">
                        <div class="one_player" v-for="(option, index) in corr_ans" :key="index" :index="index">
                            <input type="text" readonly="readonly" :placeholder="'Верный ответ'" :value="decorate_short_ans(option)" class="wait_users" @click="show_answer(option, corr_ans, index)">
                            <div id="delete_player" @click="corr_ans.pop(index)">x</div>
                        </div>
                        <div id="plus_player" @click="corr_ans.push('')">+</div>
                    </div>
                </div>
                <div style="width:30%;height:50%;float:left;margin-left:12.5%;margin-top:3%;border:2px solid silver;border-radius:0.75vw;">
                    <div style="text-align:center;font-size: 1.65vw;">Прочие варианты ответов</div>
                    <hr>
                    <div class="scroll" style="height:70%;width:100%;">
                        <div class="one_player" v-for="(option, index) in ans" :key="index" :index="index">
                            <input readonly="readonly" :placeholder="'Вариант ответа'" :value="decorate_short_ans(option)" class="wait_users" @click="show_answer(option, ans, index)">
                            <div id="delete_player" @click="ans.pop(index)">x</div>
                        </div>
                        <div id="plus_player" @click="ans.push('')">+</div>
                    </div>
                </div>
                <div id="question_preview" v-show="full_question">
                    <div style="float:right;margin-top:1%;margin-right:1%;font-size:1.35vw;" @click="close_with_save">X</div>
                    <div id="wording_header" resize="false" style="text-align: center;margin-left:0;">Формулировка ответа</div>
                    <textarea rows="12" id="answer" style="margin-left:5%;width:90%;height:70%;" :value="current_ans"></textarea>
                </div>
                <div id="media_preview" v-show="media_preview">
                    <div style="float:right;margin-top:1%;margin-right:1%;font-size:1.35vw;" @click="media_preview = false">X</div>
                    <img id="media_preview_content" v-if="!selected_question.media" src="@/assets/no_image.svg.png" style="width:80%;height:80%;margin-left:10%;margin-top:2.5%;"/>
                    <img id="media_preview_content" v-else-if="selected_question.media.slice(-3) == 'png'" :src="get_preview_media()" style="width:80%;height:80%;margin-left:10%;margin-top:2.5%;"/>
                    <video id="media_preview_content" v-else-if="selected_question.media.slice(-3) == 'mp4'" style="width:80%;height:80%;margin-left:10%;margin-top:2.5%;" controls="controls" :src="get_preview_media()"/>
                    <div class="button" style="padding-top:1.05%;padding-bottom:1.05%;width:21.15%;margin-left:18%;" @click="upload_media">Загрузить</div>
                    <div class="button" style="padding-top:1.05%;padding-bottom:1.05%;width:21.15%;margin-left:18%;" @click="delete_media">Удалить</div>
                </div>
            </div>
            
            <div class="delete_window_group_button">
                <div @click="save_question" class="button">Сохранить</div>
                <div @click="del_question" class="button">Удалить</div>
            </div>
            
        </div>
        <div v-if="draw === 'delete_question'" class="delete_product_window" id="delete_product_window">
            <div id="delete_product_warning">
                Вы действительно хотите удалить {{ "\"" + selected_question.shortText + "\"" }}?
            </div>
            <div class="delete_window_group_button">
                <div @click="delete_question" class="button">Да</div>
                <div @click="cancel_question" class="button">Нет</div>
            </div>
        </div>
    </div>
</template>

<script>
import { SRC_PATH, SERVER_PATH } from '../common_const.js'
import { parse } from 'date-fns';

export default {
  name: 'QuestionsList',
  props:['selected_product', 'products', 'current_round', 'first_round_products', 'questions_to_relation'],
  data(){
    return {
            selected_question: null,
            draw: 'questions',
            is_media: false,
            new_version_product: {},
            type_maping: new Map([ ["TEXT", "Без выбора ответа"], ["AUCTION", "Вопрос-аукцион"], ["TEXT_WITH_ANSWERS", "С выбором ответа"], ["MEDIA", "Вопрос с медиа фрагментом"], ]),
            invert_type_maping: new Map([ ["Без выбора ответа", "TEXT"], ["Вопрос-аукцион", "AUCTION"], ["С выбором ответа", "TEXT_WITH_ANSWERS"], ["Вопрос с медиа фрагментом", "MEDIA"], ]),
            corr_ans: [],
            ans: [],
            full_question: false,
            current_ans_arr: null,
            current_ans_index: null,
            current_ans: null,
            media_preview: false,
            relation_map: new Map([['Не выбрано', null]]),
            displayed_question_type: ""
    }
  },
  methods: {
        upload_media: async function () {
            var input = document.createElement('input');
            input.type = 'file';

            input.onchange = async e =>  { 
                let file = e.target.files[0];
                let formData = new FormData();
                formData.set('uploads', file);
                const regex = /media\d+.\w+/g;
                const found = document.getElementById("media_preview_content").src.match(regex);
                const current_type = found ? found[0].slice(-3) : "png";
                const file_type = file.type.slice(-3);
                if(!this.selected_question.media)
                {
                    await fetch(SERVER_PATH + "/question/media/create/" + this.selected_question.questionId, {
                    method: "POST",
                    body: formData
                    }).then( this.$emit('reload-question') );
                    if( current_type == file_type )
                        document.getElementById("media_preview_content").src = SRC_PATH + "media" + this.selected_question.questionId + file_type;
                    else
                        this.media_preview = false;
                }
                else
                {
                    await fetch(SERVER_PATH + "/question/media/update/" + this.selected_question.questionId, {
                    method: "POST",
                    body: formData
                    }).then( this.$emit('reload-question') );
                    if( current_type == file_type )
                        document.getElementById("media_preview_content").src = SRC_PATH + "media" + this.selected_question.questionId + file_type;
                    else
                        this.media_preview = false;
                }
            }
            input.click();
        },
        delete_media: async function () {
            fetch(SERVER_PATH + "/question/media/delete/" + this.selected_question.questionId, {
                    method: "DELETE",
                    });
            this.media_preview = false;
            this.selected_question.media = null;
        },
        upload_logo: async function () {
            const uploadFileEle = document.getElementById("selected_logo");
            let file = uploadFileEle.files[0];
            let formData = new FormData();
            formData.set('uploads', file);
            if(!this.selected_product.logo)
            {
                await fetch(SERVER_PATH + "/product/logo/create/" + this.selected_product.productId, {
                method: "POST",
                body: formData
                });
                document.getElementById("logo_preview").src = this.get_preview();
            }
            else
            {
                await fetch(SERVER_PATH + "/product/logo/update/" + this.selected_product.productId, {
                method: "POST",
                body: formData
                });
                document.getElementById("logo_preview").src = this.get_preview();
            }
        },
        get_preview: function () {
            if(!this.selected_product.logo)
                return SRC_PATH + 'empty_logo.png' + '?' + Date.now();
            const regex = /logo\d+.\w+/g;
            const found = this.selected_product.logo.match(regex);
            return SRC_PATH + found + '?' + Date.now();
        },
        get_preview_media: function () {
            if(!this.selected_question.media)
                return null;
            const regex = /media\d+.\w+/g;
            const found = this.selected_question.media.match(regex);
            return SRC_PATH + found + '?' + Date.now();
        },
        decorate_short_ans: function (full_ans) {
            if(full_ans.length > 20)
                return full_ans.slice(0, 20) + '...';
            return full_ans;
        },
        close_with_save: function () {
            this.full_question = false;
            this.current_ans_arr[this.current_ans_index] = document.getElementById('answer').value;
        },
        show_answer: function (ans, arr, index) {
            this.full_question = true;
            this.current_ans = ans;
            this.current_ans_arr = arr;
            this.current_ans_index = index;
        },
        get_displayed_type: function () {
            return this.type_maping.get(this.selected_question.type);
        },
        get_db_type: function (user_type) {
            return this.invert_type_maping.get(user_type);
        },
        reset: function () {
            if(document.getElementById('color_switcher'))
            {
                this.new_version_product.color = 'color:white;background:' + document.getElementById('avatar').style.backgroundColor;
                let rgb2hex=c=>'#'+c.match(/\d+/g).map(x=>(+x).toString(16).padStart(2,0)).join``;
                document.getElementById('color_switcher').value = rgb2hex(document.getElementById('avatar').style.backgroundColor);
                this.new_version_product.name = this.selected_product.name;
            }
            if(document.getElementById('wording'))
            {
                this.selected_question.shortText = this.selected_product.questions[this.selected_question.index].shortText;
                this.selected_question.text = this.selected_product.questions[this.selected_question.index].text;
                this.selected_question.type = this.selected_product.questions[this.selected_question.index].type;
                this.selected_question.answers = this.selected_product.questions[this.selected_question.index].answers;
            }
        },
        save_new_name: function () {
            var color = String(document.getElementById('color_switcher').value);
            var name = document.getElementById("edit_window_input").value;
            this.new_version_product.color = 'color:white;background:' + color;
            this.new_version_product.name = name;
        },
        save_new_color: function () {
            var color = String(document.getElementById('color_switcher').value);
            this.new_version_product.color = 'color:white;background:' + color;
        },
        update_is_media: function () {
            if(document.getElementById('type_selector'))
            {
                this.selected_question.shortText = document.getElementById('short_name').value;
                this.selected_question.type = this.get_db_type(document.getElementById('type_selector').value);
                this.selected_question.text = document.getElementById('wording').value;
                this.is_media = document.getElementById('type_selector').value == 'Вопрос с медиа фрагментом';
            }
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
            fetch(SERVER_PATH + "/product/delete/" + String(this.selected_product[2]), {
                method: "DELETE",
                headers: {'Content-Type': 'application/json'}
                });
        },
        to_questions: function () {
            this.reset();
            this.draw = 'questions';
        },
        save_edit: function () {
            this.$emit('final-edit-product', this.new_version_product.name, this.new_version_product.color);
            var edit = document.getElementById("edit");
            var click_delete = document.getElementById("delete");
            click_delete.style.opacity = 1;
            edit.style.opacity = 1;
        },
        choose_question: function (index) {
            this.draw = 'edit_question';
            this.selected_question = JSON.parse(JSON.stringify(this.selected_product.questions[index]));
            this.selected_question.index = index;
            this.is_media = this.selected_question.type === "MEDIA";
            this.corr_ans = JSON.parse(this.selected_question.answers).CorrectAnswers;
            this.ans = JSON.parse(this.selected_question.answers).AllAnswers.filter(item => !this.corr_ans.includes(item));
        },
        del_question: function () {
            this.draw = 'delete_question';
        },
        delete_question: function () {
            this.draw = 'questions';
            this.$emit('delete-question', this.selected_question.questionId);
        },
        cancel_question: function () {
            this.draw = 'edit_question';
        },
        save_question: function () {
            this.selected_question.answers = JSON.stringify({AllAnswers:this.ans.concat(this.corr_ans), CorrectAnswers:this.corr_ans});
            this.$emit('edit-question', this.selected_question);
            this.draw = 'questions';
        },
        add_question: function () {
            this.$emit('add-question');
        },
        check_delete_product: function () {
            this.draw = 'delete';
        },
        edit_product: function () {
            this.draw = 'edit';
            this.new_version_product.color = this.selected_product.colour;
            this.new_version_product.name = this.selected_product.name;
        },
        save_edit_changes: function () {
            this.selected_question.shortText = document.getElementById('short_name').value;
            this.selected_question.type = this.get_db_type(document.getElementById('type_selector').value);
            this.selected_question.text = document.getElementById('wording').value;
        },
        beautify_date: function (date) {
            let a = parse(date, 'M/d/yyyy hh:mm:ss aa', new Date());
            return a.toLocaleString('ru-RU');
        },
        change_relation: function () {
            fetch(SERVER_PATH + "/product/relation/update", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({  FirstProduct: this.relation_map.get(document.getElementById("relation").value),
                                        SecondProduct: this.selected_product.productId})
        })
        },
        change_relation_question: function () {
        //     fetch(SERVER_PATH + "/product/relation/update", {
        //         method: "POST",
        //         headers: {'Content-Type': 'application/json'},
        //         body: JSON.stringify({  FirstProduct: this.relation_map.get(document.getElementById("relation").value),
        //                                 SecondProduct: this.selected_product.productId})
        // })
        },
  },
  mounted: function() {
    this.first_round_products.forEach(product => this.relation_map.set(product.name, product.productId));
  },
  updated: function() {
    if(document.getElementById('relation'))
    {
        fetch(SERVER_PATH + "/product/relation/get/" + this.selected_product.productId, {
                method: "GET",
                headers: {'Content-Type': 'application/json'},
                }).then(res => res.json()).then(data => document.getElementById('relation').value = data.name);
    }
    if(document.getElementById('question_relation'))
    {
        fetch(SERVER_PATH + "/question/relation/get/" + this.selected_question.questionId, {
                method: "GET",
                headers: {'Content-Type': 'application/json'},
                }).then(res => res.json()).then(data => document.getElementById('question_relation').value = data.shortText);
    }
    if(document.getElementById('type_selector'))
    {
        this.displayed_question_type = this.get_displayed_type();
    }
  }
}

document.addEventListener("DOMNodeInserted", function () {
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
    width: 75%;
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
    width: 80%;
    height: 100%;
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
    /* text-align: center; */
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
    /* margin-top: 5.5%; */
    text-align: left;
    margin-left: 2%;
    width: 50%;
    border-bottom: 3px solid black;
    border-top: none;
    border-left: none;
    border-right: none;
    outline:none;
    font-size: 2.5vw;
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
    width: 100%;
    font-size: 2.5vw;
    text-align: center;
}

.delete_window_group_button{
    width: 80%;
}

.button{
    float: left;
    width: 26.6%;
    background-color: green;
    text-align: center;
    padding-top: 1.33%;
    padding-bottom: 1.33%;
    color: white;
    font-size: 1.1vw;
    font-weight: bold;
    border-radius: 20px;
    margin-left: 23.4%;
    margin-top: 1.75%;
}

.button:hover{
    box-shadow: 0 0 10px 100px orange inset;
}

#edit_question{
    height: 40%;
}

#type_selector, #question_relation{
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
    width: 80%;
    height: 50%;
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
    margin-top: 2%;
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

.wait_users{
    margin-top: 2%;
    margin-left: 10%;
    width: 75%;
    border: solid black 0.15vw;
    border-radius: 10px;
    text-align: center;
    font-size: 1.3vw;
    padding-top: 1.5%;
    padding-bottom: 1.5%;
    float: left;
}

#delete_player{
    float: left;
    width: 5%;
    margin-top: 4%;
    margin-left: 2%;
    font-size: 1.3vw;
}

#plus_player{
    margin-top: 2%;
    margin-left: 10%;
    width: 80%;
    text-align: center;
    font-size: 1.5vw;
    font-weight: bold;
    float: left;
}

hr{
    width: 90%;
    color: silver;
    float: left;
    margin-left: 5%;
    margin-right: 5%;
}

#question_preview{
  position: absolute;
  background-color: white;
  border: 0.15vw solid black;
  left: 27.5%;
  width: 45%;
  top: 30%;
  height: 40%;
  border-radius: 1vw;
}

#media_preview{
  position: absolute;
  background-color: white;
  border: 0.15vw solid black;
  left: 12.5%;
  width: 75%;
  top: 20%;
  height: 75%;
  border-radius: 1vw;
}

#selected_logo{
    opacity: 0;
}

#selected_media{
    opacity: 0;
}

#media_preview_content:hover{
    transform: scale(1); /* (150% zoom - Note: if the zoom is too large, it will go outside of the viewport) */
}

.cell{
    float: left;
    width: 25%;
    margin-bottom: 2.5%;
    display: flex;
    flex-direction: column;
    justify-content: space-around;
    align-items: center;
    text-align: center;
}

.weight{
    width: 50%;
    height: 0.35vmax;
    margin-bottom: 2.5%;
    float: left;
}
</style>
