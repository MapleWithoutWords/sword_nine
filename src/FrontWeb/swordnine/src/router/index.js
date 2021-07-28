import Vue from 'vue'
import VueRouter from 'vue-router'

const Login = () =>
  import(/* webpackChunkName:"login_home_welcome" */ '../views/Login.vue')
const Home = () =>
  import(/* webpackChunkName:"login_home_welcome" */ '../views/Home.vue')
const Welcome = () =>
  import(/* webpackChunkName:"login_home_welcome" */ '../views/Welcome.vue')

const dataSourceIndex = () =>
  import(
    /* webpackChunkName:"dataSource" */ '../views/dataSource/dataSourceIndex.vue'
  )
const classAttributeIndex = () =>
  import(
    /* webpackChunkName:"dataSource" */ '../views/dataSource/classAttributeIndex.vue'
  )
const modelManagement = () =>
  import(
    /* webpackChunkName:"dataSource" */ '../views/dataSource/modelManagement.vue'
  )
const parentClassAttributeIndex = () =>
  import(
    /* webpackChunkName:"dataSource" */ '../views/dataSource/parentClassAttributeIndex.vue'
  )
const ruleIndex = () =>
  import(
    /* webpackChunkName:"dataSource" */ '../views/codeBI/ruleIndex.vue'
  )
const templateIndex = () =>
  import(
    /* webpackChunkName:"dataSource" */ '../views/codeBI/templateIndex.vue'
  )
const codeGenIndex = () =>
  import(
    /* webpackChunkName:"dataSource" */ '../views/codeBI/codeGenIndex.vue'
  )

Vue.use(VueRouter)

const routes = [
  {
    path: '/',
    redirect: '/home'
  },
  {
    path: '/login',
    component: Login
  },
  {
    path: '/home',
    component: Home,
    children: [
      {
        path: '/welcome',
        component: Welcome
      },
      {
        path: '/dataSourceIndex',
        component: dataSourceIndex
      },
      {
        path: '/classAttributeIndex',
        component: classAttributeIndex
      },
      {
        path: '/modelManagement',
        component: modelManagement
      },
      {
        path: '/parentClassAttributeIndex',
        component: parentClassAttributeIndex
      },
      {
        path: '/ruleIndex',
        component: ruleIndex
      },
      {
        path: '/templateIndex',
        component: templateIndex
      },
      {
        path: '/codeGenIndex',
        component: codeGenIndex
      }
    ]
  }
]

const router = new VueRouter({
  routes
})
router.beforeEach((to, form, next) => {
  const token = window.sessionStorage.getItem('token')
  if (to.path === '/login') {
    if (token) {
      return next('/home')
    }
  }

  // if (to.path !== '/login') {
  //   if (token === undefined || token === null) {
  //     return next('/login')
  //   }
  // }
  return next()
})

export default router
