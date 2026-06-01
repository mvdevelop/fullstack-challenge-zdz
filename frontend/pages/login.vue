<template>
  <v-app>
    <v-main>
      <v-container class="fill-height" fluid>
        <v-row align="center" justify="center">
          <v-col cols="12" sm="8" md="5" lg="4">
            <v-card rounded="xl" class="pa-2">
              <v-card-text class="pa-6">
                <div class="text-center mb-6">
                  <v-avatar color="primary" size="56" class="mb-3">
                    <v-icon icon="mdi-wallet" size="28" color="white" />
                  </v-avatar>
                  <h5 class="font-weight-bold mb-1">FinanceiroApp</h5>
                  <p class="text-medium-emphasis text-body-2 mb-0">Controle suas finanças de forma simples</p>
                </div>

                <v-tabs v-model="tab" centered class="mb-5" bg-color="transparent" color="primary">
                  <v-tab value="login" class="font-weight-medium">Entrar</v-tab>
                  <v-tab value="register" class="font-weight-medium">Criar conta</v-tab>
                </v-tabs>

                <v-window v-model="tab">
                  <v-window-item value="login">
                    <v-form @submit.prevent="fazerLogin">
                      <v-text-field v-model="loginForm.email" label="Email" type="email" prepend-inner-icon="mdi-email" variant="outlined" density="comfortable" rounded="lg" class="mb-2" :rules="[v => !!v || 'Email obrigatório']" />
                      <v-text-field v-model="loginForm.senha" label="Senha" type="password" prepend-inner-icon="mdi-lock" variant="outlined" density="comfortable" rounded="lg" class="mb-4" :rules="[v => !!v || 'Senha obrigatória']" />
                      <v-btn block size="large" color="primary" rounded="lg" :loading="loading" type="submit">Entrar</v-btn>
                    </v-form>
                  </v-window-item>
                  <v-window-item value="register">
                    <v-form @submit.prevent="fazerRegistro">
                      <v-text-field v-model="registerForm.nome" label="Nome" prepend-inner-icon="mdi-account" variant="outlined" density="comfortable" rounded="lg" class="mb-2" :rules="[v => !!v || 'Nome obrigatório']" />
                      <v-text-field v-model="registerForm.email" label="Email" type="email" prepend-inner-icon="mdi-email" variant="outlined" density="comfortable" rounded="lg" class="mb-2" :rules="[v => !!v || 'Email obrigatório']" />
                      <v-text-field v-model="registerForm.senha" label="Senha" type="password" prepend-inner-icon="mdi-lock" variant="outlined" density="comfortable" rounded="lg" class="mb-4" :rules="[v => !!v || 'Senha obrigatória', v => v.length >= 6 || 'Mínimo 6 caracteres']" />
                      <v-btn block size="large" color="primary" rounded="lg" :loading="loading" type="submit">Criar conta</v-btn>
                    </v-form>
                  </v-window-item>
                </v-window>

                <v-alert v-if="erro" type="error" variant="tonal" density="compact" class="mt-4" rounded="lg" closable @click:close="erro = ''">{{ erro }}</v-alert>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
const auth = useAuthStore()
const router = useRouter()
const tab = ref('login')
const loading = ref(false)
const erro = ref('')
const loginForm = reactive({ email: '', senha: '' })
const registerForm = reactive({ nome: '', email: '', senha: '' })

async function fazerLogin() {
  loading.value = true; erro.value = ''
  try { await auth.login(loginForm.email, loginForm.senha); router.push('/') }
  catch (e: any) { erro.value = e.message || 'Erro ao fazer login' }
  finally { loading.value = false }
}

async function fazerRegistro() {
  loading.value = true; erro.value = ''
  try { await auth.register(registerForm.nome, registerForm.email, registerForm.senha); router.push('/') }
  catch (e: any) { erro.value = e.message || 'Erro ao criar conta' }
  finally { loading.value = false }
}
</script>