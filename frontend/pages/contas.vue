<template>
  <div>
    <div class="d-flex align-center justify-space-between mb-4">
      <div>
        <div class="text-caption text-medium-emphasis font-weight-medium">Saldo total</div>
        <div class="text-h4 font-weight-bold" :class="saldoTotal >= 0 ? 'text-success' : 'text-error'">
          {{ formatarValor(saldoTotal) }}
        </div>
      </div>
      <v-btn color="primary" prepend-icon="mdi-plus" @click="abrirDialog()">Nova conta</v-btn>
    </div>

    <v-row>
      <v-col v-for="conta in contas" :key="conta.id" cols="12" sm="6" md="4">
        <v-card rounded="lg" :opacity="conta.ativa ? 1 : 0.5">
          <v-card-text>
            <div class="d-flex align-center gap-3 mb-3">
              <v-avatar :color="corTipo(conta.tipo)" variant="tonal" size="44" rounded="lg">
                <v-icon :icon="iconeTipo(conta.tipo)" size="22" />
              </v-avatar>
              <div>
                <div class="font-weight-medium">{{ conta.nome }}</div>
                <div class="text-caption text-medium-emphasis">{{ conta.tipo }} · {{ conta.moeda }}</div>
              </div>
              <v-spacer />
              <v-menu>
                <template #activator="{ props }">
                  <v-btn icon="mdi-dots-vertical" size="small" variant="text" v-bind="props" />
                </template>
                <v-list density="compact">
                  <v-list-item prepend-icon="mdi-pencil" title="Editar" @click="abrirDialog(conta)" />
                  <v-list-item prepend-icon="mdi-delete" title="Desativar" @click="excluir(conta.id)" />
                </v-list>
              </v-menu>
            </div>
            <div class="text-h5 font-weight-bold" :class="conta.saldoAtual >= 0 ? 'text-success' : 'text-error'">
              {{ formatarValor(conta.saldoAtual) }}
            </div>
            <div class="text-caption text-medium-emphasis mt-1">
              Saldo inicial: {{ formatarValor(conta.saldoInicial) }}
            </div>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <!-- Dialog criar/editar -->
    <v-dialog v-model="dialog" max-width="440" persistent>
      <v-card rounded="lg">
        <v-card-title class="pa-6 pb-2">{{ editando ? 'Editar conta' : 'Nova conta' }}</v-card-title>
        <v-card-text class="pa-6 pt-3">
          <v-text-field v-model="form.nome" label="Nome da conta" class="mb-3" />
          <v-select
            v-model="form.tipo"
            label="Tipo"
            :items="['Corrente', 'Poupanca', 'Cartao', 'Investimento', 'Outro']"
            class="mb-3"
          />
          <v-text-field
            v-model.number="form.saldoInicial"
            label="Saldo inicial"
            prefix="R$"
            type="number"
            :disabled="!!editando"
            class="mb-3"
          />
          <v-select v-model="form.moeda" label="Moeda" :items="['BRL', 'USD', 'EUR']" />
        </v-card-text>
        <v-card-actions class="pa-6 pt-0">
          <v-spacer />
          <v-btn variant="text" @click="dialog = false">Cancelar</v-btn>
          <v-btn color="primary" variant="flat" :loading="loading" @click="salvar">
            {{ editando ? 'Salvar' : 'Criar' }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup lang="ts">
import type { Conta, ContaForm } from '~/types'

definePageMeta({ layout: 'default', middleware: 'auth' })

const { contas, loading, saldoTotal, buscar, criar, atualizar, excluir } = useContas()

const dialog = ref(false)
const editando = ref<Conta | null>(null)
const form = reactive<ContaForm>({ nome: '', tipo: 'Corrente', saldoInicial: 0, moeda: 'BRL' })

function formatarValor(v: number) {
  return `R$ ${v.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}`
}

function corTipo(tipo: string) {
  return { Corrente: 'primary', Poupanca: 'success', Cartao: 'error', Investimento: 'warning', Outro: 'secondary' }[tipo] ?? 'primary'
}

function iconeTipo(tipo: string) {
  return { Corrente: 'mdi-bank', Poupanca: 'mdi-piggy-bank', Cartao: 'mdi-credit-card', Investimento: 'mdi-chart-line', Outro: 'mdi-wallet' }[tipo] ?? 'mdi-wallet'
}

function abrirDialog(conta?: Conta) {
  editando.value = conta ?? null
  if (conta) Object.assign(form, { nome: conta.nome, tipo: conta.tipo, saldoInicial: conta.saldoInicial, moeda: conta.moeda })
  else Object.assign(form, { nome: '', tipo: 'Corrente', saldoInicial: 0, moeda: 'BRL' })
  dialog.value = true
}

async function salvar() {
  if (editando.value) await atualizar(editando.value.id, form)
  else await criar(form)
  dialog.value = false
}

onMounted(buscar)
</script>
