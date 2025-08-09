import React from 'react';
import "@fortawesome/fontawesome-svg-core/styles.css";
        import { config } from "@fortawesome/fontawesome-svg-core";
        config.autoAddCss = false; // Prevent Font Awesome from adding its own CSS
import Header from './header'
import Footer from './footer'


export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <>
      <Header />
        <main>{children}</main>
      <Footer />
    </>
    );
}
