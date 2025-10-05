'use client'
import React, { ChangeEvent, useEffect, useState } from 'react'
import jQuery from 'jquery';
import axios from 'axios';
import Image from 'next/image';

const mfaapi = axios.create({
  baseURL: "https://localhost:7292",
  headers: {'Accept': 'application/json',
          'Content-Type': 'multipart/form-data',
          'Authorization': 'inherit'},
})

const api = axios.create({
  baseURL: "https://localhost:7292",
  headers: {'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': 'inherit'},
})

interface Profiledata {
  firstname: string,
  lastname: string,
  email: string,
  mobile: string,
  qrcodeurl: string,
  profilepic: string

  user: Profiledata;
}

interface Updateprofiledata {
  statuscode: number,
  message: string,
  qrcodeurl: string
}

export default function Profile() {
  const [firstname, setFirstname] = useState<string>('');
  const [lastname, setLastname] = useState<string>('');
  const [email, setEmail] = useState<string>('');
  const [mobile, setMobile] = useState<string>('');
  const [password, setPassword] = useState<string>('');  
  const [confpassword, setConfpassword] = useState<string>('');
  const [message, setMessage] = useState<string>('');
  const [userid, setUserid] = useState<string>('');
  const [profilepic, setProfilepic] = useState<string>('');
  const [qrcode, setQrcode] = useState<string>('');
  const [token, setToken] = useState<string>('');
  const [showsave, setShowsave] = useState<boolean>(false);
  // const [selectedFile, setSelectedFile] = useState<File | null>(null);

  const fetchData = async (id: string, token: string) => {
    setMessage("Pls wait, retrieving data..")

    api.get<Profiledata>(`/api/getbyid/${id}`,{headers: {
      Authorization: `Bearer ${token}`
    }})
    .then(res => {
        const data: Profiledata = res.data.user;
        setLastname(data.lastname); 
        setFirstname(data.firstname); 
        setEmail(data.email);
        setMobile(data.mobile);
        if (data.qrcodeurl !== null) {
          setQrcode(data.qrcodeurl)
        } else {
          setQrcode('/images/qrcode.png');
        }
        if (data.profilepic !== null) {
          setProfilepic(data.profilepic);
        } else {
          setProfilepic('/users/pix.png')
        }
      },(error) => {        
        setMessage(error.response.data.message);
        setTimeout(() => {
            setMessage('');
        }, 1000);
      });    
  }
  
  useEffect(() => {
    jQuery("#cpwd").hide();
    jQuery("#qcode").hide();
    jQuery("#qcode-info").hide();
    const idno = window.sessionStorage.getItem('USERID');
    if (idno !== null) {
      setUserid(idno);
    }

    const tkn = window.sessionStorage.getItem('TOKEN');
    if (tkn !== null) {
      setToken(tkn);
    } 
    if (userid.length > 0) {
      fetchData(userid, token);      
    }
    setMessage('');
  },[userid, token]);

  const updateProfile =  (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault(); 
    if (password !== confpassword) {
      setMessage("New Password does not matched.");
      window.setTimeout(() => {
        setMessage("");
      }, 3000);
      return;
    }
    const data =JSON.stringify({ lastname: lastname, firstname: firstname, mobile: mobile });
    api.patch<Updateprofiledata>(`/api/updateprofile/${userid}`, data, { headers: {
        Authorization: `Bearer ${token}`
    }})
    .then((res) => {
        const data: Updateprofiledata = res.data;
        if (data.statuscode === 200) {
            setMessage(data.message);
            setTimeout(() => {
              setMessage('');
            },800);
        } else {
          setMessage(data.message);
          setTimeout(() => {
            setMessage('');
          },800);
        }
    }, (error) => {
        setMessage(error.response.data.message);
        setTimeout(() => {
          setMessage('');
        },3000);
        return;
    });
  }

  const changePwd = () => {
    if (jQuery('#chgPwd').is(":checked")) {    
      jQuery('#mfacode').prop('checked', false);
      jQuery("#qcode-info").hide();
      jQuery("#qcode").hide();
      jQuery("#cpwd").show();
      setShowsave(true);
    } 
    else {
      jQuery("#cpwd").hide();
      setShowsave(false);
      setPassword('');
      setConfpassword('');
    }
  }

  const enableMFA = () => {
    if (jQuery('#mfacode').is(":checked")) {    
      setShowsave(true);
      // setQrcode('');
      if (jQuery('#chgPwd').is(":checked")) {    
        setPassword("");
        setConfpassword("");
        jQuery('#chgPwd').prop('checked', false);
        jQuery("#cpwd").hide();
      }
      jQuery("#qcode").show();
      jQuery("#qcode-info").show();
    } else {
      setShowsave(false);
      jQuery("#qcode").hide();
      jQuery("#qcode-info").hide();      
    }
  }

  const changeProfilepic = (event: ChangeEvent<HTMLInputElement>) => {
    event.preventDefault();
    const file = event.target.files?.[0] || null;

    if (file) {
      const pix = URL.createObjectURL(file);
      jQuery('#userpic').attr('src', pix);
      const formData = new FormData();
      formData.append('Id', userid);
      formData.append('Profilepic', file);
      mfaapi.post<Updateprofiledata>(`/api/uploadpicture`, formData, {headers: {
          Authorization: `Bearer ${token}`
      }})
      .then((res) => {
          const data: Updateprofiledata = res.data;
          if (data.message != null) {
              setMessage(data.message);
              setTimeout(() => {
                setMessage('');
              },3000);
              return;
          }
      }, (error) => {
        setMessage(error.response.data.message);
          setTimeout(() => {
            setMessage('');
          },3000);
          return;
      });
    }
  }

  const activateMFA =  () => {
    const data =JSON.stringify({ Twofactorenabled: true });
    api.patch<Updateprofiledata>(`/api/enablemfa/${userid}`, data, {headers: {
        Authorization: `Bearer ${token}`
    }})
    .then((res) => {
        const data: Updateprofiledata = res.data;
        setMessage(data.message);
        setTimeout(() => {
              setQrcode(data.qrcodeurl);
              setMessage('');
          },3000);
    }, (error) => {
      setMessage(error.response.data.message);
        setTimeout(() => {
          setMessage('');
        },3000);
        return;
    });    
  }

  const deactivateMFA = () => {
    const data =JSON.stringify({ Twofactorenabled: false });      
    api.patch<Updateprofiledata>(`/api/enablemfa/${userid}`, data, {headers: {
        Authorization: `Bearer ${token}`
    }})
    .then((res: any) => {
        const data: Updateprofiledata = res.data;
        setMessage(data.message);
        setTimeout(() => {
            setQrcode('/images/qrcode.png');
            setMessage('');
        },3000);
    }, (error) => {
      setMessage(error.response.data.message);            
        setTimeout(() => {
          setMessage('');
        },3000);
        return;
    });    
  }

  const changePassword = () => {
    if (password === '') {
      setMessage("Please enter new Pasword.");
        setTimeout(() => {
            setMessage('');
        },3000);
        return;
    }
    if (confpassword === '') {
      setMessage("Please enter new Pasword confirmation.");
        setTimeout(() => {
            setMessage('');
        },3000);
        return;            
    }

    if (password !== confpassword) {
      setMessage("new Password does not matched.");
        setTimeout(() => {
            setMessage('');
        },3000);
        return;            
    }

    const data =JSON.stringify({ "Password": password });      
    api.patch<Updateprofiledata>(`/api/updatepassword/${userid}`, data, {headers: {
        Authorization: `Bearer ${token}`
    }})
    .then((res) => {
        const data: Updateprofiledata = res.data;
        if (data.message != null) {
            setMessage(data.message);
            setTimeout(() => {
              setMessage('');
            },3000);
            return;
        }
    }, (error) => {
      setMessage(error.response.data.message);
        setTimeout(() => {
            setMessage('');
        },3000);
        return;
    });        
}

  return (
    <div className='container'>
      <div className="card mt-2 mb-10">
        <div className='card-header bg-primary text-white'>
             <strong>USER PROFILE ID NO.</strong>&nbsp; {userid}
        </div>
        <div className="card-body">

          <form onSubmit={updateProfile} encType='multipart/form-data' autoComplete='off' method='POST'>
            <div className='row'>
              <div className='col'>
                <div className="mb-3">
                  <label htmlFor="firstname" className="form-label">First Name</label>
                  <input type="text" className="form-control input-border" value={firstname} onChange={e => setFirstname(e.target.value)}/>
                </div>            
                <div className="mb-3">
                  <label htmlFor="lastname" className="form-label">Last Name</label>
                  <input type="text" className="form-control input-border" value={lastname} onChange={e => setLastname(e.target.value)}/>
                </div>            
              </div>
              <div className='col'>
                    <div className="mb-3 mt-2">
                      <div className='user-pix-box'>
                        <Image id="userpic" src={profilepic} fill={true} className='userprofile' alt={''}/>
                      </div>
                      <input type="file" onChange={e => changeProfilepic(e)} className="form-control form-control-sm" id="profilepic"/>
                    </div>                                    
              </div>
            </div>

            <div className='row'>
              <div className='col'>
                <div className="mb-3">
                  <label htmlFor="email" className="form-label">Email Address</label>
                  <input type="email" readOnly className="form-control input-border" value={email} onChange={e => setEmail(e.target.value)}/>
                </div>            
              </div>
              <div className='col'>
                <div className="mb-3">
                  <label htmlFor="mobile" className="form-label">Mobile No.</label>
                  <input type="text" className="form-control input-border" value={mobile} onChange={e => setMobile(e.target.value)}/>
                </div>            
              </div>              
            </div>

            <div className='row'>
              <div className='col'>

                  <div className="form-check">
                    <input className="form-check-input cb-border" type="checkbox" value="" onChange={changePwd} id="chgPwd"/>
                    <label className="form-check-label" htmlFor="flexCheckDefault">
                        Change Password
                    </label>
                  </div>
                  <div id="cpwd">
                     <div className="mb-3">
                       <input type="password" className="form-control" value={password} onChange={e => setPassword(e.target.value)} placeholder='enter new password.'/>
                     </div>            
                     <div className="mb-3">
                       <input type="password" className="form-control" value={confpassword} onChange={e => setConfpassword(e.target.value)} placeholder='enter new password confirmation.'/>
                     </div>            
                     <button onClick={changePassword} type="button" className='btn btn-primary'>change password</button>
                  </div> 
                  <div id="qcode">
                    {
                      qrcode !== '' ?
                        <Image width={200} height={200} className='qrcode2' src={qrcode} alt='' />
                    :
                        <Image width={200} height={200} className='qrcode1' src="/images/qrcode.png" alt='' />
                    }
                  </div>
              </div>
              <div className='col'>
                <div className="form-check">
                  <input className="form-check-input cb-border" type="checkbox" value="" onChange={enableMFA} id="mfacode"/>
                  <label className="form-check-label" htmlFor="flexCheckDefault">
                      2-Factor Authentication
                  </label>
                </div>
                <div id="qcode-info">
                    <p className='text-danger'><strong>Requirements</strong></p>
                    <p>You need to install <strong>Google or Microsoft Authenticator</strong> in your Mobile Phone, once installed, click Enable Button below, and <strong>SCAN QR CODE</strong>, next time you login, another dialog window will appear, then enter the <strong>OTP CODE</strong> from your Mobile Phone in order for you to login.</p>
                    <div className='row'>
                      <div className='col-2'>
                        <button onClick={activateMFA} type="button" className='btn btn-primary'>Enable</button>
                      </div>
                      <div className='col-2'>
                        <button onClick={deactivateMFA} type="button" className='btn btn-secondary'>Disable</button>                        
                      </div>
                    </div>
                </div>
              </div>              
            </div>
            { showsave === false ? (
              <button type='submit' className='btn btn-primary mt-3'>save</button>
            ):
                null
            }
          </form>    
        </div>
        <div className='card-footer'>
          <div className='w-100 text-danger'>{message}</div>
        </div>
      </div>
      <br/><br/>

    </div>
  )
}