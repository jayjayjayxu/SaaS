<template>
  <div class="login-page">
    <el-card class="login-form-container">
      <template #header>
        <div class="card-header">
          <h1>登录</h1>
        </div>
      </template>

      <el-form @submit.prevent="handleLogin" label-position="top">
        <el-form-item label="用户名">
          <el-input v-model="username" placeholder="请输入用户名" size="large" clearable />
        </el-form-item>
        <el-form-item label="密码">
          <el-input v-model="password" type="password" placeholder="请输入密码" size="large" show-password />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" native-type="submit" class="login-button" size="large">登 录</el-button>
        </el-form-item>
      </el-form>
      <div class="register-link">
        没有账户？ <RouterLink to="/register">立即创建</RouterLink>
      </div>
    </el-card>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import axios from 'axios'; // 导入axios
import { useRouter } from 'vue-router'; // 导入useRouter用于页面跳转
import { useAuthStore } from '@/stores/authStore'; // 导入我们的authStore

const authStore = useAuthStore(); // 获取store实例
const username = ref('');
const password = ref('');
const router = useRouter(); // 获取路由实例

const handleLogin = async () => {
  try {
    await authStore.login(username.value, password.value);
    // 成功后的跳转逻辑已经在store的action里了
  } catch (error: any) {
    // 从store的action里捕获错误并提示用户
    alert(`登录失败: ${error.response?.data || '请检查网络或联系管理员'}`);
  }
};
</script>

<style scoped>
.login-page {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  background-color: #f0f2f5;
  font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
}

.login-form-container {
  width: 100%;
  max-width: 400px;
  padding: 48px;
  background-color: white;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  text-align: center;
}

h1 {
  font-size: 24px;
  font-weight: 600;
  margin-bottom: 8px;
  color: #202124;
}

p {
  margin-bottom: 24px;
  color: #5f6368;
}

.input-group {
  margin-bottom: 16px;
  text-align: left;
}

input {
  width: 100%;
  padding: 14px 16px;
  font-size: 16px;
  border: 1px solid #dadce0;
  border-radius: 4px;
  box-sizing: border-box; /* 确保padding不会撑大宽度 */
  transition: border-color 0.2s;
}

input:focus {
  outline: none;
  border-color: #1a73e8;
  box-shadow: 0 0 0 2px rgba(26, 115, 232, 0.2);
}

.button-group {
  margin-top: 24px;
}

.login-button {
  width: 100%;
  padding: 14px 16px;
  font-size: 16px;
  font-weight: 600;
  color: white;
  background-color: #1a73e8;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.2s;
}

.login-button:hover {
  background-color: #185abc;
}
.register-link {
  margin-top: 24px;
  font-size: 14px;
  color: #5f6368;
}

.register-link a {
  color: #1a73e8;
  font-weight: 600;
  text-decoration: none;
}

.register-link a:hover {
  text-decoration: underline;
}
</style>