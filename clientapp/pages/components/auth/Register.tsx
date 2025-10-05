'use client'
import React, { useState } from 'react'
import axios from 'axios';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faAddressCard } from '@fortawesome/free-solid-svg-icons'; 

const api = axios.create({
  baseURL: "https://localhost:7292",
  headers: {'Accept': 'application/json',
            'Content-Type': 'application/json'}
})

interface Registerdata {
  statuscode: number,
  message: string
}

export default function Register() {
    const [firstname, setFirstname] = useState<string>('');
    const [lastname, setLastname] = useState<string>('');
    const [email, setEmail] = useState<string>('');
    const [mobile, setMobile] = useState<string>('');
    const [username, setUsername] = useState<string>('');
    const [password, setPassword] = useState<string>('');
    const [message, setMessage] = useState<string>('');

    const closeRegistration = () => {
      setFirstname("");
      setLastname("");
      setEmail("");
      setMessage("");
      setMobile("");
      setUsername("");
      setPassword("");
    }

    const submitRegistration = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault(); 
        const data =JSON.stringify({ lastname: lastname, firstname: firstname,email: email, mobile: mobile,
          username: username, password: password });
        api.post<Registerdata>("/signup", data)
        .then((res) => {
            const data: Registerdata = res.data;
              setMessage(data.message);
              window.setTimeout(() => {
                setMessage('');
              }, 3000);
          }, (error) => {
            setMessage(error.response.data.message);
                window.setTimeout(() => {
                  setMessage('');
                }, 3000);
        });
    }

  return (
<div className="modal fade" id="staticRegister" data-bs-backdrop="static" data-bs-keyboard="false" tabIndex={-1} aria-labelledby="staticRegisterLabel" aria-hidden="true">
  <div className="modal-dialog modal-dialog-centered">
    <div className="modal-content">
      <div className="modal-header bg-danger">
        <h1 className="modal-title fs-5 text-white" id="staticRegistgerLabel"><FontAwesomeIcon icon={faAddressCard}/>&nbsp;Account Registration</h1>
        <button onClick={closeRegistration} type="button" className="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <div className="modal-body">
        
      <form onSubmit={submitRegistration}>

            <div className="row">
             <div className="col">
                <div className="mb-3">
                    <input type="text" required className="form-control" value={firstname} onChange={e => setFirstname(e.target.value)} autoComplete='off' placeholder="enter First Name"/>
                </div>           
              </div>
              <div className="col">  
                <div className="mb-3">
                    <input type="text" required className="form-control" value={lastname} onChange={e => setLastname(e.target.value)} autoComplete='off' placeholder="enter Last Name"/>
                </div>                          
              </div>
            </div>

            <div className="row">
             <div className="col">
                <div className="mb-3">
                    <input type="email" required className="form-control" value={email} onChange={e => setEmail(e.target.value)} autoComplete='off' placeholder="enter Email Address"/>
                </div>           
              </div>
              <div className="col">  
                <div className="mb-3">
                    <input type="text" required className="form-control" value={mobile} onChange={e => setMobile(e.target.value)} autoComplete='off' placeholder="enter Mobile No."/>
                </div>                          
              </div>
            </div>


            <div className="row">
             <div className="col">
                <div className="mb-3">
                    <input type="text" className="form-control" value={username} onChange={e => setUsername(e.target.value)} autoComplete='off' placeholder="enter Username"/>
                </div>           
              </div>
              <div className="col">  
                <div className="mb-3">
                    <input type="text" className="form-control" value={password} onChange={e => setPassword(e.target.value)} autoComplete='off' placeholder="enter Password"/>
                </div>                          
              </div>
            </div>
            <button type="submit" className="btn btn-danger text-white mx-2">register</button>
            <button type="reset" className="btn btn-danger text-white mx-2">reset</button>

        </form>

      </div>
      <div className="modal-footer">
            <div id="registerMsg" className="w-100 text-left text-danger">{message}</div>
      </div>
    </div>
  </div>
</div>

  )
}