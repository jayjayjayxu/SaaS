import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import TaskDetailView from '../views/TaskDetailView.vue' 
import { useAuthStore } from '@/stores/authStore'; 

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
      meta: { requiresAuth: true } // 1. 我们给首页路由添加一个元信息，标记它需要认证
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView
    },
    {
      path: '/register',
      name: 'register',
      component: RegisterView
    },
    {
      path: '/projects/:projectId/tasks/:taskId', // :projectId 和 :taskId 是动态参数
      name: 'task-detail',
      component: TaskDetailView,
      meta: { requiresAuth: true } // 这个页面也需要登录
    }
  ]
})

// --- ▼▼▼ 全局前置路由守卫 ▼▼▼ ---
router.beforeEach((to, from, next) => {
  // 在路由守卫之外，我们不能直接使用useAuthStore()
  // 但在守卫内部可以，因为此时Pinia已经被激活
  const authStore = useAuthStore();

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    // 如果页面需要认证，但用户未登录（通过getter判断）
    next({ name: 'login' }); // 重定向到登录页
  } else {
    next(); // 其他情况，直接放行
  }
});

export default router