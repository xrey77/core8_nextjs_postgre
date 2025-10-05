'use client'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faUnlock } from '@fortawesome/free-solid-svg-icons'; 
import React, { useState } from 'react'
import Mfa from './Mfa';
import jQuery from 'jquery';
import axios from 'axios';

const api = axios.create({
  baseURL: "https://localhost:7292",
  headers: {'Accept': 'application/json',
            'Content-Type': 'application/json'}
})

interface Users {
  id: string,
  firstname: string,
  lastname: string,
  username: string,
  roles: string,  
  isactivated: string,
  isblocked: string,
  profilepic: string,
  qrcodeurl: string,
  token: string,
  statuscode: number,
  message: string
}

export default function Login() {
   const [username, setUsername] = useState<string>('');
   const [password, setPassword] = useState<string>('');
   const [loginmessage, setLoginMessage] = useState<string>('');
   const [dizable, setDizable] = useState<boolean>(false);
  
    const submitLogin =  (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();       
        setDizable(true);
        setLoginMessage("Please wait..");
        const data =JSON.stringify({ username: username, password: password });
        api.post<Users>("/signin", data)
        .then(async (res) => {
                const data: Users = res.data;
                setLoginMessage(data.message);
                if (data.qrcodeurl !== null) {
                    window.sessionStorage.setItem('USERID',data.id);
                    window.sessionStorage.setItem('TOKEN',data.token);
                    window.sessionStorage.setItem('ROLE',data.roles);
                    window.sessionStorage.setItem('USERPIC',data.profilepic);
                    jQuery("#loginReset").trigger('click');
                    jQuery("#mfamodal").trigger('click')         
                } else {
                      window.sessionStorage.setItem('USERID',data.id);
                      window.sessionStorage.setItem('USERNAME',data.username);
                      window.sessionStorage.setItem('TOKEN',data.token);                        
                      window.sessionStorage.setItem('ROLE',data.roles);
                      window.sessionStorage.setItem('USERPIC',data.profilepic);
                      window.location.reload();
                }
                setTimeout(() => {
                  setDizable(false);
                  setLoginMessage('');
                }, 3000);
          }, (error) => {
                setLoginMessage(error.response.data.message);
                setTimeout(() => {
                  setDizable(false);
                  setLoginMessage('');
                }, 3000);
                return;
        });
    }

    const closeLogin = () => {
      setUsername("");
      setPassword("");
      window.sessionStorage.clear();
    }

  return (
    <>
<Mfa/>
<div className="modal fade" id="staticLogin" data-bs-backdrop="static" data-bs-keyboard="false" tabIndex={-1} aria-labelledby="staticLoginLabel" aria-hidden="true">
  <div className="modal-dialog modal-sm modal-dialog-centered">
    <div className="modal-content">
      <div className="modal-header bg-primary">
        <h1 className="modal-title fs-5 text-white" id="staticLoginLabel"><FontAwesomeIcon icon={faUnlock}/>&nbsp;User Signin</h1>
        <button id="close" onClick={closeLogin} type="button" className="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <div className="modal-body">
        <form onSubmit={submitLogin}>
            <div className="mb-3">
                <input disabled={dizable} type="text" required className="form-control" value={username} onChange={e => setUsername(e.target.value)} autoComplete='off' placeholder="enter Username"/>
            </div>            
            <div className="mb-3">
                <input disabled={dizable} type="password" required className="form-control" value={password} onChange={e => setPassword(e.target.value)} autoComplete='off' placeholder="enter Password"/>
            </div>            

            <button disabled={dizable} type="submit" className="btn btn-primary">signin</button>
            <button type="reset" className="btn btn-primary mx-1">reset</button>
            <button id="mfamodal" type="button" className="btn btn-primary hide-mfa" data-bs-toggle="modal" data-bs-target="#staticMFA">MFA</button>

        </form>
      </div>
      <div className="modal-footer">
        <div id="loginMsg" className="w-100 text-left text-danger">{loginmessage}</div>
      </div>
    </div>
  </div>
</div>
</>
    );
}