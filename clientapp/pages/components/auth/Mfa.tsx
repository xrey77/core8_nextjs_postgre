'use client'
import React, { useState } from "react"
import axios from 'axios';

const api = axios.create({
  baseURL: "https://localhost:7292",
  headers: {'Accept': 'application/json',
            'Content-Type': 'application/json'}
})

interface Mfadata {
  username: string,
  statuscode: number,
  message: string  
}

export default function Mfa() {
    const [otpcode, setOtpcode] = useState<string>('');
    const [message, setMessage] = useState<string>('');
    const [dizable, setDizable] = useState<boolean>(false);
    
    const closeMFA = () => {
        window.sessionStorage.clear();
        window.location.href="/";
    }

    const submitMFA = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();         
        setDizable(true);
        const idno = window.sessionStorage.getItem('USERID')?.toString();
        const data =JSON.stringify({ id: idno, otp: otpcode });
        api.post<Mfadata>("/validateotp", data)
        .then((res) => {
            const data: Mfadata = res.data;
                setMessage(data.message);
                setOtpcode('');
                sessionStorage.setItem("USERNAME", data.username);
                window.setTimeout(() => {
                  setMessage('');
                  window.location.reload();
                  setDizable(false);
                }, 3000);
          }, (error) => {
               setMessage(error.response.data.message);
                window.setTimeout(() => {
                  setDizable(false);
                  setMessage('');
                  setOtpcode('');
                }, 3000);
                return;
        });    
    }



    return (
    <div className="modal fade" id="staticMFA" data-bs-backdrop="static" data-bs-keyboard="false" tabIndex={-1} aria-labelledby="staticMFALabel" aria-hidden="true">
    <div className="modal-dialog modal-sm modal-dialog-centered">
        <div className="modal-content">
        <div className="modal-header bg-warning">
            <h1 className="modal-title fs-5 text-dark" id="staticMFALabel">2-Factor Authenticator</h1>
            <button onClick={closeMFA} type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
        </div>
        <div className="modal-body">
            <form onSubmit={submitMFA}>
                <div className="mb-3">
                    <input disabled={dizable} type="text" required className="form-control" value={otpcode} onChange={e => setOtpcode(e.target.value)} autoComplete='off' placeholder="enter OTP Code"/>
                </div>            

                <button disabled={dizable} type="submit" className="btn btn-warning">submit</button>
            </form>
        </div>
        <div className="modal-footer">
            <div id="MfaMsg" className="w-100 text-left text-danger">{message}</div>
        </div>
        </div>
    </div>
    </div>
    );
}