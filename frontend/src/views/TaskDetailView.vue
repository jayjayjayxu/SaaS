<template>
  <div class="task-detail-container">
    <div v-if="loading" class="loading">正在加载任务详情...</div>
    <div v-else-if="task" class="task-content">
      <div class="task-header">
        <h1 :class="{ completed: task.isCompleted }">{{ task.title }}</h1>
        <span :class="['status-badge', task.isCompleted ? 'status-completed' : 'status-pending']">
          {{ task.isCompleted ? '已完成' : '待处理' }}
        </span>
      </div>
      <p class="task-description">{{ task.description || '这个任务没有详细描述。' }}</p>
      <div class="actions">
        <button 
          @click="toggleCompletionStatus"
          :class="['action-button', task.isCompleted ? 'reopen-btn' : 'complete-btn']"
        >
          {{ task.isCompleted ? '标记为未完成' : '完成此任务' }}
        </button>
      </div>
      <router-link to="/" class="back-link">&larr; 返回项目列表</router-link>
    </div>
    <div v-else class="error">
      无法找到该任务，或加载失败。
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router'; 
import apiClient from '@/services/apiClient';
import { WatchDirectoryFlags } from 'typescript';

const route = useRoute();
const router = useRouter(); 
const task = ref<any>(null); // 初始为null
const loading = ref(true);
const errorMsg = ref(''); // 用于存储错误信息

onMounted(async () => {
  // 从路由参数中获取ID
  const projectId = route.params.projectId;
  const taskId = route.params.taskId;

  // 检查ID是否存在
  if (!projectId || !taskId) {
      errorMsg.value = '无效的URL，缺少项目或任务ID。';
      loading.value = false;
      return;
  }

  try {
    // 调用真实的API来获取任务详情
    const response = await apiClient.get(`/projects/api/projects/${projectId}/tasks/${taskId}`);
    task.value = response.data;
  } catch (error: any) {
    console.error('获取任务详情失败:', error);
    errorMsg.value = error.response?.data || '无法加载任务详情，请重试。';
  } finally {
    loading.value = false;
  }
});
const toggleCompletionStatus = async () => {
  if (!task.value) return;
  // 同样使用“乐观更新”
  task.value.isCompleted = !task.value.isCompleted;
  try {
    await apiClient.patch(`/projects/api/projects/${route.params.projectId}/tasks/${task.value.id}/toggle`);
        if(task.value.isCompleted == true){
          await sleep(1000);
      router.back();
    }
  } catch (error) {
    console.error('更新任务状态失败:', error);
    alert('抱歉，更新任务失败，请重试。');
    task.value.isCompleted = !task.value.isCompleted;
  }
};
function sleep(ms: number) {
  return new Promise(resolve => setTimeout(resolve, ms));
}
</script>

<style scoped>
.task-detail-container {
  max-width: 800px;
  margin: 40px auto;
  padding: 2rem;
  background-color: #fff;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.1);
}
.task-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid #eee;
  padding-bottom: 1rem;
  margin-bottom: 1.5rem;
}
h1 {
  font-size: 2rem;
  margin: 0;
}
h1.completed {
  text-decoration: line-through;
  color: #888;
}
.status-badge {
  padding: 4px 12px;
  border-radius: 12px;
  font-weight: 600;
  font-size: 0.8rem;
}
.status-pending {
  background-color: #fff0c2;
  color: #7a4f01;
}
.status-completed {
  background-color: #e9f7ef;
  color: #1e8449;
}
.task-description {
  font-size: 1.1rem;
  line-height: 1.6;
  color: #333;
}
.back-link {
  display: inline-block;
  margin-top: 2rem;
  color: #1a73e8;
  text-decoration: none;
  font-weight: 600;
}
.back-link:hover {
  text-decoration: underline;
}
.loading, .error {
  text-align: center;
  padding: 3rem;
  color: #888;
}
/* 在 TaskDetailView.vue 的 <style scoped> 中添加 */

.actions {
  margin-top: 2rem;
  padding-top: 1.5rem;
  border-top: 1px solid #eee;
  text-align: right;
}

.action-button {
  border: none;
  padding: 10px 20px;
  font-size: 0.9rem;
  border-radius: 5px;
  color: white;
  font-weight: 600;
  cursor: pointer;
  transition: background-color 0.2s;
}

.complete-btn {
  background-color: #28a745; /* 绿色 */
}
.complete-btn:hover {
  background-color: #218838;
}

.reopen-btn {
  background-color: #ffc107; /* 黄色 */
  color: #212529;
}
.reopen-btn:hover {
  background-color: #e0a800;
}

.back-link {
  /* 让返回链接和按钮分开 */
  display: block;
  float: left;
  margin-top:2.5rem;
}
</style>