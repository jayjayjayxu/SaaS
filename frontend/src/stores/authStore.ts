import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import apiClient from '@/services/apiClient';

// 使用defineStore创建一个store，第一个参数是这个store的唯一ID
export const useAuthStore = defineStore('auth', () => {
  const router = useRouter();

  // --- State ---
  // 从localStorage初始化token，这样刷新页面后登录状态不会丢失
  const token = ref(localStorage.getItem('jwt_token'));
  // 将来我们可以在这里存储用户信息
  // const user = ref(null); 

  // --- Getters ---
  // 像Vue组件里的computed属性，它会根据state的变化自动更新
  const isAuthenticated = computed(() => !!token.value);

  // --- Actions ---
  // 登录操作
  const login = async ( username:string, password:string) => {
    try {
      const response = await apiClient.post('/identity/api/auth/login', { username, password });
      const newToken = response.data.token;
      if (newToken) {
        token.value = newToken;
        localStorage.setItem('jwt_token', newToken);
        apiClient.defaults.headers.common['Authorization'] = `Bearer ${newToken}`;
        await router.push({ name: 'home' }); // 登录成功后跳转到主页
        return true;
      }
    } catch (error) {
      console.error('Login action failed:', error);
      throw error; // 将错误抛出，让组件可以捕获
    }
    return false;
  };

  // 登出操作
  const logout = () => {
    token.value = null;
    localStorage.removeItem('jwt_token');
    delete apiClient.defaults.headers.common['Authorization'];
    router.push({ name: 'login' }); // 登出后跳转回登录页
  };

  // 注册操作
  const register = async (username:string, password:string) => {
    try {
      await apiClient.post('/identity/api/auth/register', { username, password });
      return true; // 注册成功返回true
    } catch (error) {
      console.error('Register action failed:', error);
      throw error; // 抛出错误
    }
  };

  // 将state, getters, actions暴露出去，这样组件才能使用它们
  return { token, isAuthenticated, login, logout, register };
});