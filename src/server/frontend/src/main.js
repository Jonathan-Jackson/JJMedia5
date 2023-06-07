import { createApp } from "vue";
import App from "./App.vue";

import PrimeVue from "primevue/config";

import InputText from "primevue/inputtext";
import Button from "primevue/button";
import Divider from "primevue/divider";
import Card from "primevue/card";
import DataTable from "primevue/datatable";
import Column from "primevue/column";
import ProgressSpinner from "primevue/progressspinner";
import Message from "primevue/message";
import Menu from "primevue/menu";

import "primevue/resources/themes/saga-blue/theme.css"; //theme
import "primevue/resources/primevue.min.css"; //core css
import "primeicons/primeicons.css"; //icons
import "primeflex/primeflex.css";

const app = createApp(App);
app.use(PrimeVue);
app.component("InputText", InputText);
app.component("Button", Button);
app.component("Divider", Divider);
app.component("Card", Card);
app.component("DataTable", DataTable);
app.component("Column", Column);
app.component("ProgressSpinner", ProgressSpinner);
app.component("Message", Message);
app.component("Menu", Menu);

app.mount("#app");
