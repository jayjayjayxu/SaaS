 <template>
  <div class="login-page">
    <div class="login-form-container">
      <h1>创建您的账户</h1>
      <p>只需一步即可开始</p>
      <form @submit.prevent="handleRegister">
        <div class="input-group">
          <input type="text" id="username" v-model="username" required placeholder="用户名">
        </div>
        <div class="input-group">
          <input type="password" id="password" v-model="password" required placeholder="密码">
        </div>
        <div class="button-group">
          <button type="submit" class="login-button">同意并创建账户</button>
        </div>
      </form>
      <div class="back-to-login-link">
        已经有账户了？ <RouterLink to="/login">返回登录</RouterLink>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import axios from 'axios'; // 导入axios
import { useRouter } from 'vue-router'; // 导入useRouter用于页面跳转
import apiClient from '@/services/apiClient';

const username = ref('');
const password = ref('');
const router = useRouter(); // 获取路由实例

const handleRegister = async () => {
  // 检查输入是否为空
  if (!username.value || !password.value) {
    alert('用户名和密码不能为空！');
    return;
  }

  try {
    // 调用后端的注册API，注意地址是通过网关的
    await apiClient.post('/identity/api/auth/register', {
      username: username.value,
      password: password.value
    });

    // 注册成功
    alert('注册成功！即将跳转到登录页面。');

    // 跳转到登录页
    await router.push({ name: 'login' });

  } catch (error: any) {
    // 处理错误
    if (error.response) {
      // 后端返回了错误信息，例如“用户名已存在”
      alert(`注册失败: ${error.response.data}`);
    } else {
      // 其他网络错误等
      alert('注册失败，请检查网络或联系管理员。');
    }
    console.error('Registration failed:', error);
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

/* 在 RegisterView.vue 的 <style scoped> 中添加 */

.back-to-login-link {
  margin-top: 24px;
  font-size: 14px;
  text-align: center;
  color: #5f6368;
}

.back-to-login-link a {
  color: #1a73e8;
  font-weight: 600;
  text-decoration: none;
}

.back-to-login-link a:hover {
  text-decoration: underline;
}
</style>