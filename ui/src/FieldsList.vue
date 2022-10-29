<template>
    <AdminHeader @logout="log_out"/>
    <NavigationButton @fields-1="to_fields_1" @fields-2="to_fields_2" @masters="to_masters" />

    <ManageMasters v-if="current_view == 'masters'"/>

    <FieldList  v-if="current_view == 'fields'" @add-field="add_field" @select-product="select_product" :products="products" />
    <AddFields  v-if="current_view == 'fields'" @close-add-field="close_add_field" @create-field="create_field"  :is_add_product="is_add_product" />

    <ProductMenu v-if="current_view == 'questions'" :current_view="current_view" :selected_product="selected_product" @to-fields="to_fields" @fields-1="to_fields_1" @fields-2="to_fields_2" @delete-product="delete_product" @edit-product="edit_product" />
    <QuestionsList v-if="current_view == 'questions'" @to-masters="to_masters"  @edit-question="edit_question" @add-question="add_question" @final-delete-product="final_delete_product" @final-edit-product="final_edit_product" @save-edit="save_edit" @reset-edit="reset_edit" :selected_product="selected_product" :draw="draw" :products="products" :cache_product="cache_product" ref="q_list" />
</template>

<script>
import AdminHeader from './components/AdminHeader.vue'
import NavigationButton from './components/NavigationButton.vue'
import FieldList from './components/FieldList.vue'
import AddFields from './components/AddFields.vue'
import ProductMenu from './components/ProductMenu.vue'
import QuestionsList from './components/QuestionsList.vue'
import ManageMasters from './components/ManageMasters.vue'
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
    ManageMasters
  }, 
  data(){
    return {
        current_text: 'Ведущие',
        is_add_product: false,
        current_view: 'fields',
        selected_product: ['СберАптека', 'background:red;color:white;', -1000, []],
        draw: 'questions',
        products: [],
        cache_product: ['name', 'colour']
    }
  },
  methods: {
        to_masters: function () {
            this.current_view = 'masters';
        },
        to_fields_1: function () {
            this.current_view = 'fields';
            let product_ref = this.products;
            product_ref.length = 0;
            fetch(SERVER_PATH + "/product/getAll", {
            method: "GET",
            headers: {'Content-Type': 'application/json'}
            }).then( res => res.json() ).then( data => data.forEach(function(item) {
                product_ref.push({ id:item.id, text: item.name, color: item.colour, questions: item.questions })}) );
        },
        to_fields_2: function () {
            this.current_view = 'fields';
            let product_ref = this.products;
            product_ref.length = 0;
            fetch(SERVER_PATH + "/product/getAll", {
            method: "GET",
            headers: {'Content-Type': 'application/json'}
            }).then( res => res.json() ).then( data => data.forEach(function(item) {
                product_ref.push({ id:item.id, text: item.name, color: item.colour, questions: item.questions })}) );
        },
        add_field: function(){
            this.is_add_product = true;
        },
            close_add_field: function(){
            this.is_add_product = false;
        },
            create_field: function(name, color){
            this.products.push({ text: name, value: this.products.lenght, color:"background:" + color, questions: [] });
        },
        select_product: function(name, color, id, questions){
            this.current_view = 'questions';
            if(name == 'Финал' || name == 'Полуфинал')
            {
                this.selected_product = [name, color + 'background-color:white;color:black;font-size:2vw;padding-top: 15%;padding-bottom: 15%;', id, questions];
                this.cache_product[1] = color + 'background-color:white;color:black;font-size:2vw;padding-top: 15%;padding-bottom: 15%;';
            }
            else
            {
                this.selected_product = [name, color + ';color:white;', id, questions];
                this.cache_product[1] = color + ';color:white;';
            }
            this.cache_product[0] = name;
        },
        delete_product: function(){
            this.$refs.q_list.check_delete_product();
        },
        edit_product: function(){
            this.$refs.q_list.edit_product();
        },
        final_delete_product: function(){
            this.products = this.products.filter(option => option.text != this.selected_product[0]);
            this.current_view = 'fields';
        },
        save_edit: function(){
            var old_name = this.selected_product[0];
            var old_id = this.selected_product[2];
            var old_questions = this.selected_product[3];
            var color = this.cache_product[1];
            var name = this.cache_product[0]
            this.selected_product = [name, color, old_id, old_questions];
            this.products.forEach(function(item) {
                    if(item.text == old_name)
                    {
                        item.text = name;
                        item.color = color;
                        item.id = old_id;
                        item.questions = old_questions;
                    }
                });
            if(this.selected_product[2])
            {
                fetch("http://api.vm-96694bec.na4u.ru/product/update", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({id:this.selected_product[2], name: this.selected_product[0], colour: this.selected_product[1], questions:this.selected_product[3]})
                })
            }
            else
            {
                fetch("http://api.vm-96694bec.na4u.ru/product/create", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({name: this.selected_product[0], colour: this.selected_product[1], questions:this.selected_product[3]})
                })
            }
        },
        reset_edit: function(){
            if( document.getElementById('color_switcher') )
            {
                let rgb2hex=c=>'#'+c.match(/\d+/g).map(x=>(+x).toString(16).padStart(2,0)).join``;
                document.getElementById('color_switcher').value = rgb2hex(document.getElementById('avatar').style.backgroundColor);
                this.cache_product[0] = this.selected_product[0];
                this.cache_product[1] = this.selected_product[1];
            }
        },
        final_edit_product: function(name, color){
            this.cache_product[0] = name;
            this.cache_product[1] = color;
        },
        edit_question: function(product, question, question_id, new_type, new_wording, short_text, answer){
            this.products.forEach(function(item) {
                if(item.text == product)
                {
                item.questions.forEach(function(item) {
                    if(question_id && item.id == question_id)
                    {
                        item.questionType = new_type;
                        item.text = new_wording;
                        item.shortText = short_text;
                        item.answer = answer;
                        return;
                    }
                    else if((question.tmp_id && question.tmp_id == item.tmp_id) || question.tmp_id == 0)
                    {
                        item.questionType = new_type;
                        item.text = new_wording;
                        item.shortText = short_text;
                        item.answer = answer;
                        return;
                    }

                });   
                }
            });
        if(this.selected_product[2])
        {
            fetch(SERVER_PATH + "/product/update", {
            method: "POST",
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify({id:this.selected_product[2], name: this.selected_product[0], colour: this.selected_product[1], questions:this.selected_product[3]})
            })
        }
        else
        {
            fetch(SERVER_PATH + "/product/create", {
            method: "POST",
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify({name: this.selected_product[0], colour: this.selected_product[1], questions:this.selected_product[3]})
            })
        }
        },
        add_question: function(name){
            let ar_ref = this.selected_product;
            this.products.forEach(function(item) {
                if(name == item.text)
                {
                    item.questions.push({ text: '', shortText:"", questionType: 'TEXT', answer: '', tmp_id: item.questions.length == 0 ? 0 : -item.questions.length });
                    ar_ref[3] = item.questions;
                }
            });
        },
        log_out: function(){
            this.$emit('logout');
        },
  }, 
  mounted: function () {
      let product_ref = this.products;
      product_ref.length = 0;
    this.$nextTick(function () {
    fetch(SERVER_PATH + "/product/getAll", {
            method: "GET",
            headers: {'Content-Type': 'application/json'}
            }).then( res => res.json() ).then( data => data.forEach(function(item) {
                product_ref.push({ id:item.id, text: item.name, color: item.colour, questions: item.questions })}) );
    fetch(SERVER_PATH + "/admin/waiting", {
            method: "GET",
            headers: {'Content-Type': 'application/json'}
            }).then( res => res.json() ).then( data => console.log(data) );
  })
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
