<template>
    <AdminHeader @logout="log_out" @change-pass="change_pass = true" @reload-question="read_product_list()"/>
    <NavigationButton @fields-1="to_fields_1" @fields-2="to_fields_2" @fields-3="to_fields_3" @masters="to_masters" @players="to_players" @challenge="to_challenge" />

    <ManageMasters v-if="current_view == 'masters'"/>
    <ManagePlayers v-if="current_view == 'players'"/>

    <ChallengeList v-if="current_view == 'challenge'" :challenge="challenge" />

    <FieldList v-if="current_view == 'fields'" @add-field="add_field" @select-product="select_product" :products="products" />
    <AddFields v-if="current_view == 'fields'" @close-add-field="close_add_field" @create-field="create_field" :is_add_product="is_add_product" />

    <ProductMenu v-if="current_view == 'questions'" :current_view="current_view" :round="current_round" :selected_product="selected_product" @fields-1="to_fields_1" @fields-2="to_fields_2" @fields-3="to_fields_3" @delete-product="delete_product" @edit-product="edit_product" />
    <QuestionsList v-if="current_view == 'questions'" @to-masters="to_masters" @add-question="add_question" @edit-question="edit_question" @delete-question="delete_question" @final-delete-product="final_delete_product" @final-edit-product="final_edit_product" @reset-edit="reset_edit" :selected_product="selected_product" :draw="draw" :products="products" :current_round="current_round" :first_round_products="first_round_products" :cache_product="cache_product" :questions_to_relation="questions_to_relation" ref="q_list" @reload-question="read_product_list()" />

    <div id="question_preview" v-show="change_pass">
        <div style="float:right;font-size:1.75vw;margin-top:0.25%;margin-right:2.5%;width:10%;text-align:right;" @click="close_changes_pass">x</div>
        <div style="width:90%;margin-left:5%;font-size:1.5vw;margin-top:2.5%;">Введите новый пароль:</div>
        <input type="password" id="new_pass" style="width:90%;margin-left:5%;font-size:1.5vw;margin-top:1%;border-bottom:0.1vw solid silver;border-top:none;border-left:none;border-right:none;outline:none;" placeholder="Новый пароль">
        <div style="width:90%;margin-left:5%;font-size:1.5vw;margin-top:2.5%;">Для подтверждения действия введите старый пароль:</div>
        <input type="password" id="old_pass" style="width:90%;margin-left:5%;font-size:1.5vw;margin-top:1%;border-bottom:0.1vw solid silver;border-top:none;border-left:none;border-right:none;outline:none;" placeholder="Старый пароль">
        <div v-show="error_change" id="change_status" style="float:left;color:red;margin-top: 2%;margin-left: 5%;font-size:1.1vw;"></div>
        <div class="button" style="float:right;" @click="update_pass">Сохранить</div>
    </div>
</template>

<script>
import AdminHeader from './components/AdminHeader.vue'
import NavigationButton from './components/NavigationButton.vue'
import FieldList from './components/FieldList.vue'
import AddFields from './components/AddFields.vue'
import ProductMenu from './components/ProductMenu.vue'
import QuestionsList from './components/QuestionsList.vue'
import ManageMasters from './components/ManageMasters.vue'
import ManagePlayers from './components/ManagePlayers.vue'
import ChallengeList from './components/ChallengeList.vue'
import { SERVER_PATH } from './common_const.js'

export default {
  name: 'FieldsList',
  components: {
    AdminHeader,
    NavigationButton,
    FieldList,
    AddFields,
    ProductMenu,
    QuestionsList,
    ManageMasters,
    ManagePlayers,
    ChallengeList
  }, 
  data(){
    return {
        current_text: 'Ведущие',
        is_add_product: false,
        current_view: 'fields',
        current_round: 1,
        selected_product: null,
        draw: 'questions',
        products: [],
        cache_product: ['name', 'colour'],
        change_pass: false,
        error_change: false,
        first_round_products: [],
        questions_to_relation: []
    }
  },
  methods: {
        update_pass: function () {
            fetch(SERVER_PATH + "/user/update/password", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({Login: JSON.parse(localStorage.getItem('user')).login, NewPassword: document.getElementById('new_pass').value, OldPassword: document.getElementById('old_pass').value})
                }).then(res => {
                    if(res.status !== 200)
                    {
                        this.error_change = true;
                        res.text().then(text => this.set_error_message(text));
                    }
                    else
                    {
                        this.error_change = true;
                        res.text().then(this.set_error_message("Пароль успешно обновлён"));
                    }
                });
        },
        set_error_message: function (message) {
            document.getElementById("change_status").innerHTML = message;
        },
        close_changes_pass: function () {
            this.change_pass = false;
            document.getElementById('new_pass').value = '';
            document.getElementById('old_pass').value = '';
            this.error_change = false;
        },
        to_masters: function () {
            this.current_view = 'masters';
        },
        to_players: function () {
            this.current_view = 'players';
        },
        to_challenge: function () {
            this.current_view = 'challenge';
        },
        to_fields_1: function () {
            this.current_view = 'fields';
            this.current_round = 1;
            this.read_product_list();
        },
        to_fields_2: function () {
            this.current_view = 'fields';
            this.current_round = 2;
            this.read_product_list();
        },
        to_fields_3: function () {
            this.current_view = 'fields';
            this.current_round = 3;
            this.read_product_list();
        },
        add_field: function(){
            this.is_add_product = true;
        },
        close_add_field: function(){
            this.is_add_product = false;
        },
        create_field: async function(name, color){
            await fetch(SERVER_PATH + "/product/create", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({name: name, colour: "background:" + color + ";color:white;", questions: [], logo: null, round: this.current_round})
                })
            this.read_product_list();
        },
        select_product: function(selected){
            this.current_view = 'questions';
            this.selected_product = selected;
            this.selected_product.questions.forEach(question => {
                fetch(SERVER_PATH + "/question/weight/get", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({ProductId: null, QuestionId: question.questionId, Weight: null})
            }).then(res => res.text()).then(data => {
                if(data == 0)
                    question.weight = 'rgb(0, 255, 0);';
                else if(data <= 50)
                    question.weight = `rgb(${255 * data / 50}, 255, 0);`;
                else
                    question.weight = `rgb(255, ${255 - 255 * (data - 50) / 50}, 0);`;
                });
            });
            if(this.selected_product.round != 3)
                return;
            fetch(SERVER_PATH + "/product/relation/get/" + this.selected_product.productId, {
                method: "GET",
                headers: {'Content-Type': 'application/json'},
                }).then(res => res.json()).then(data => {
                    this.first_round_products.forEach(product => {
                    if(product.productId == data.id)
                        this.questions_to_relation = product.questions;
                    });
            });
        },
        delete_product: function(){
            this.$refs.q_list.check_delete_product();
        },
        edit_product: function(){
            this.$refs.q_list.edit_product();
        },
        final_delete_product: async function(){
            this.current_view = 'fields';
            await fetch(SERVER_PATH + "/product/delete/" + String(this.selected_product.productId), {
                method: "DELETE",
                headers: {'Content-Type': 'application/json'}
                });
            this.read_product_list();
        },
        reset_edit: function(old_version){
            if( document.getElementById('color_switcher') )
            {
                let rgb2hex=c=>'#'+c.match(/\d+/g).map(x=>(+x).toString(16).padStart(2,0)).join``;
                document.getElementById('color_switcher').value = rgb2hex(document.getElementById('avatar').style.backgroundColor);
                this.selected_product.name = old_version.name;
                this.selected_product.colour = old_version.color;
            }
        },
        final_edit_product: async function(name, color){
            this.selected_product.name = name;
            this.selected_product.colour = color;
            await fetch(SERVER_PATH + "/product/update", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify(this.selected_product)
                });
            this.read_product_list();
        },
        edit_question: async function(new_question){
            this.selected_product.questions[new_question.index] = new_question;
            delete this.selected_product.index;
            await fetch(SERVER_PATH + "/product/update", {
            method: "POST",
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(this.selected_product)
            });
            this.read_product_list();
        },
        add_question: async function(){
            this.selected_product.questions.push({answers:'', type:'TEXT_WITH_ANSWERS', shortText:'', text:'', media: null});
            await fetch(SERVER_PATH + "/product/update", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify(this.selected_product)
                });
            this.read_product_list();
        },
        delete_question: async function(id){
            await fetch(SERVER_PATH + "/question/delete/" + String(id), {
                method: "DELETE",
                headers: {'Content-Type': 'application/json'},
                });
            this.read_product_list();
        },
        log_out: function(){
            this.$emit('logout');
        },
        read_product_list: function(){
            fetch(SERVER_PATH + "/product/get/all/" + String(this.current_round), {
            method: "GET",
            headers: {'Content-Type': 'application/json'}
            }).then( res => res.json() ).then( data => this.fill_product_list(data) );
        },
        fill_product_list: function(data){
            this.products = data;
            this.products.forEach(product => {
                fetch(SERVER_PATH + "/question/weight/get", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({ProductId: product.productId, QuestionId: null, Weight: null})
            }).then(res => res.text()).then(data => {
                if(data == 0)
                    product.weight = 'rgb(0, 255, 0);';
                else if(data <= 50)
                    product.weight = `rgb(${255 * data / 50}, 255, 0);`;
                else
                    product.weight = `rgb(255, ${255 - 255 * (data - 50) / 50}, 0);`;
                });
            });
            if(this.selected_product)
                this.update_selected();
        },
        update_selected: function(){
            for(let i = 0; i < this.products.length; ++i)
            {
                if(this.products[i].productId == this.selected_product.productId)
                    this.selected_product = this.products[i];
            }
        },
  }, 
  created: function () {
      this.read_product_list();
      fetch(SERVER_PATH + "/product/get/all/1", {
            method: "GET",
            headers: {'Content-Type': 'application/json'}
            }).then( res => res.json() ).then( data => this.first_round_products = data );
  },
    beforeCreate: async function () {
        if(!localStorage.getItem('user'))
          this.log_out();
        await fetch(SERVER_PATH + "/fields", {
                method: "GET",
                headers: {'Content-Type': 'application/json', "Authorization": "Bearer " + JSON.parse(localStorage.getItem('user')).authorizationToken}
        }).then(res => {
            if(res.status === 401 || res.status === 403)
                this.log_out();
        });
    },
    beforeMount: async function () {
        if(!localStorage.getItem('user'))
          this.log_out();
      await fetch(SERVER_PATH + "/templates", {
                method: "GET",
                headers: {'Content-Type': 'application/json', "Authorization": "Bearer " + JSON.parse(localStorage.getItem('user')).authorizationToken}
        }).then(res => {
            if(res.status === 401 || res.status === 403)
                this.log_out();
        });
  },
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
#question_preview{
  position: absolute;
  background-color: white;
  border: 0.2vw solid black;
  left: 27.5%;
  width: 45%;
  top: 30%;
  height: 40%;
  border-radius: 1vw;
}

.button{
    width: 35%;
    float: left;
    margin-top: 2%;
    margin-right: 4%;
    margin-left: 4%;
    background-color: green;
    color: #ffffff;
    font-size: 1.2vw;
    font-weight: bold;
    border-radius: 35px;
    padding-top: 1%;
    padding-bottom: 1%;
    text-align: center;
}

.button:hover {
    box-shadow: 0 0 10px 100px orange inset;
}

/* .scroll{
    height: 100%;
    overflow: auto;
    -ms-overflow-style: none;
    scrollbar-width: none;
}

.scroll::-webkit-scrollbar {
    width: 0;
    height: 0;
} */
</style>
