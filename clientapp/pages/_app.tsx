import type { AppProps } from 'next/app'
import 'bootstrap/dist/css/bootstrap.min.css'
import '@fortawesome/fontawesome-svg-core/styles.css'
import { config } from "@fortawesome/fontawesome-svg-core";
import 'jquery/dist/jquery.js'
import '../styles/globals.css';
import Layout from './layout/layout';
config.autoAddCss = false; // Prevent Font Awesome from adding its own CSS

const App = ({ Component, pageProps }: AppProps) => {
  return (
    <Layout>      
        <Component {...pageProps} />
    </Layout>
  )
}

export default App;



