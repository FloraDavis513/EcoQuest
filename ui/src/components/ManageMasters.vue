<template>
    <MasterList ref="masters" @master-chosen="chose_master" :masters="masters" :master_chosen="master_chosen" />
    <MasterRequests @go-back="go_back" @add-master="add_master" @remove-master="remove_master" @save-edit="save_edit" :master_chosen="master_chosen" :selected="selected" :fio="fio" :login="login" />
</template>

<script>
import MasterList from './MasterList.vue'
import MasterRequests from './MasterRequests.vue'
import { SERVER_PATH } from '../common_const.js'

export default {
  name: 'ManageMasters',
  data(){
    return {
      current_text: 'Игровые поля',
      master_chosen: false,
      selected: null,
      fio: null,
      login: null,
      masters:[
        { text: 'Ведущий 1', value: '1' },
        { text: 'Ведущий 2', value: '2' },
        { text: 'Ведущий 3', value: '3' },
        { text: 'Ведущий 4', value: '4' },
        { text: 'Ведущий 5', value: '5' },
        { text: 'Ведущий 6', value: '6' },
        { text: 'Ведущий 7', value: '7' },
        { text: 'Ведущий 8', value: '8' },
        { text: 'Ведущий 9', value: '9' },
        { text: 'Ведущий 10', value: '10' },
        { text: 'Ведущий 11', value: '11' },
        { text: 'Ведущий 12', value: '12' },
        ],

    }
  },
  components: {
    MasterList,
    MasterRequests
  }, 
  methods: {
    to_masters: function () {
          this.$emit('to-masters');
      },
      to_fields_1: function () {
          this.$emit('to-fields');
      },
    chose_master: function (master_chosen, selected) {
        this.master_chosen = master_chosen;
        this.selected = selected;
        this.fio = selected.getAttribute("name");
        this.login = selected.getAttribute("name");
        
    },
    go_back: function () {
        this.master_chosen = false;
        this.$refs.masters.unselect();
    },
    add_master: function(name, val){
      this.masters.push({text: name, value:val});
    },
    remove_master: function(value){
      this.masters = this.masters.filter(option => option.value != value);
      this.master_chosen = false;
      this.$refs.masters.unselect();
    },
    set_draw: function(draw){
      this.draw = draw;
    },
    log_out: function(){
        this.$emit('logout');
    },
    save_edit: function(fio){
      var name = this.selected.getAttribute("name");
      this.fio = fio;
      this.login = fio;
      this.masters.forEach(function(item) {
          if(name == item.text)
          {
              item.text = fio;
          }
      });
    },
  },
  mounted: function () {
      let masters_ref = this.masters;
      masters_ref.length = 0;
    this.$nextTick(function () {
    fetch(SERVER_PATH + "/admin/leaders", {
            method: "GET",
            headers: {'Content-Type': 'application/json'}
            }).then( res => res.json() ).then( data => data.forEach(function(item) {
                masters_ref.push(item)}) );
    });
  }
}
</script>

<style scoped>
html {
    height: 100%;
    width: 100%;
    overflow: auto;
    -ms-overflow-style: none;
    scrollbar-width: none;
    font-family: 'Roboto Light', sans-serif;
}

body {
    width: 99%;
    height: 90%;
}
#app{
  height: 100%;
    width: 100%;
}

#team_1{
  position: absolute;
  left: 1%;
}

#team_2{
  position: absolute;
  left: 74%;
}

#team_3{
  position: absolute;
  left: 1%;
  top: 63%;
}

#team_4{
  position: absolute;
   top: 63%;
  left: 74%;
}
</style>
