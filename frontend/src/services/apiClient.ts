import axios from 'axios';

// 创建一个axios实例，并配置基础URL为我们的API网关地址
const apiClient = axios.create({
  baseURL: 'http://localhost:7077', // 你的API网关地址
});

// ▼▼▼ 添加一个请求拦截器（Request Interceptor） ▼▼▼
apiClient.interceptors.request.use(
  config => {
    // 在每个请求发送前，都来这里检查一下
    const token = localStorage.getItem('jwt_token');
    if (token) {
      // 如果本地存储中有Token，就把它加到请求的Authorization头里
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  error => {
    // 处理请求错误
    return Promise.reject(error);
  }
);
// --- ▼▼▼ 添加下面这个新的“响应拦截器” ▼▼▼ ---
apiClient.interceptors.response.use(
  // 对成功的响应，我们什么都不做，直接返回
  response => response,

  // 对失败的响应，我们进行全局处理
  error => {
    // 检查错误响应是否存在，并且状态码是否是401 (Unauthorized)
    if (error.response && error.response.status === 401) {
      // 清除本地存储中无效的/过期的Token
      localStorage.removeItem('jwt_token');

      // 弹出提示，并强制刷新页面以重定向到登录页
      // 路由守卫会因为找不到Token而自动跳转到/login
      alert('您的登录已过期，请重新登录。');
      window.location.href = '/login'; 
    }

    // 将错误继续传递下去，以便组件内部的catch块可以处理其他逻辑
    return Promise.reject(error);
  }
);


export default apiClient;