import { useEffect } from 'react'
import { config } from '@fortawesome/fontawesome-svg-core'
import '@fortawesome/fontawesome-svg-core/styles.css'
import 'bootstrap/dist/css/bootstrap.min.css'
import 'jquery/dist/jquery.js'
import '../styles/globals.css'
// import type { AppProps } from 'next/app'
import Layout from './layout/layout'

config.autoAddCss = false

const App = ({ Component, pageProps }) => {
  useEffect(() => {
    require('bootstrap/dist/js/bootstrap.bundle.js');
   },[])


  return(
    <Layout>
      <Component {...pageProps} />      
    </Layout>
  ) 
}

export default App;