import type { AppProps } from 'next/app'
import 'bootstrap/dist/css/bootstrap.min.css'
import '@fortawesome/fontawesome-svg-core/styles.css'
import { config } from "@fortawesome/fontawesome-svg-core";
import './globals.css';
// import "@/styles/globals.css";

import Layout from './layout/layout';
import dynamic from 'next/dynamic';

const JQueryComponent = dynamic(() => import('./components/JQueryComponent'), {
  ssr: false,
});
config.autoAddCss = false;

const App = ({ Component, pageProps }: AppProps) => {
  return (
    <>
    <JQueryComponent/>
    <Layout>      
        <Component {...pageProps} />
    </Layout>
    </>
  )
}

export default App;



