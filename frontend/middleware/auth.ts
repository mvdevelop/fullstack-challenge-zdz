export default defineNuxtRouteMiddleware(async (to) => {
  if (to.path === '/login') return

  const auth = useAuthStore()

  // Wait for auth init to finish
  if (auth.loading) {
    await new Promise<void>((resolve) => {
      const unwatch = watch(() => auth.loading, (loading) => {
        if (!loading) {
          unwatch()
          resolve()
        }
      })
    })
  }

  if (!auth.isAuthenticated) {
    return navigateTo('/login')
  }
})