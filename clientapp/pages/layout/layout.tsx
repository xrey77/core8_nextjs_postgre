'use client'
import React from 'react';
import BootstrapClient from '../components/BootstrapClient';
import Header from './header'


export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <>
      <Header />
        <main>{children}</main>
          <BootstrapClient />        
    </>
    );
}
