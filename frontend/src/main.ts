import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'

// 1. 确保这两个import存在
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'

import './assets/main.css'

const app = createApp(App)

app.use(createPinia())
app.use(router)

// 2. 确保这行代码存在，并且在 app.mount 之前
app.use(ElementPlus)

app.mount('#app')