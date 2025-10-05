'use client';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faList, faMapLocation, faCircleQuestion, faAddressCard, faUnlock } from '@fortawesome/free-solid-svg-icons'; 
import  Link  from 'next/link';
import Image from 'next/image';
// import '../../styles/globals.css';
import Login from '../components/auth/Login';
import Register from '../components/auth/Register';
import { useEffect, useState } from 'react';
import $ from 'jquery';

export default function Header({}) {
  const [username, setUsername] = useState<string>('');
  const [profilepic, setProfilepic] = useState<string>('');
  

  useEffect(() => {
    const cookie1 = window.sessionStorage.getItem('USERNAME');
    if (cookie1 === null) {
      setUsername('');
    } else {
      setUsername(cookie1);
    }
    const cookie2 = window.sessionStorage.getItem('USERPIC');
    if (cookie2 === null) {
      setProfilepic('')
    } else {
      setProfilepic(cookie2)
    }
  },[]);

  const logout = () => {
    window.sessionStorage.removeItem('USERNAME');
    window.sessionStorage.removeItem('USERPIC');
    window.sessionStorage.removeItem('TOKEN');
    window.sessionStorage.removeItem('ROLE');
    window.location.href="/";
  }

  const servicesDropdownHide = () => {
    $('#nav1b').hide();
  }

  const productDropdown = () => {
    $('#nav1b').hide();
    $('#nav2b').show();
  }

  const productDropdownHide = () => {
    $('#nav2b').hide();
  }
  
  const productDropleave = () => {
      $('#nav2b').hide();
      $('.dropdown-menu').hide();
  }


  return(
    <div>
    <nav className="navbar navbar-expand-lg bg-dark">
      <div className="container-fluid">
        <Link className="navbar-brand" href={"/"}><Image className='logo' src={"/images/logo.png"} width={30} height={30} alt={"logo"} /></Link>
        <button className="navbar-toggler" type="button"  data-bs-toggle="offcanvas" data-bs-target="#offcanvasMenu" aria-controls="offcanvasWithBothOptions">
          <span className="navbar-toggler-icon"><Image className='burger' src={'/images/bar.png'} width={30} height={30} alt={""} /></span>
        </button>
        <div className="collapse navbar-collapse" id="navbarSupportedContent">
          <ul className="navbar-nav me-auto mb-2 mb-lg-0">
            <li className="nav-item">
              <Link  onMouseEnter={servicesDropdownHide} className="nav-link active text-white" aria-current="page" href={"/aboutus"}><FontAwesomeIcon icon={faCircleQuestion}/>&nbsp;About Us</Link>
            </li>
            <li className="nav-item dropdown">
              <Link onMouseEnter={productDropdown} className="nav-link dropdown-toggle text-white active" href={"#"} role="button" data-bs-toggle="dropdown" aria-expanded="false">
              <FontAwesomeIcon icon={faList}/>&nbsp;Products
              </Link>
              <ul id="nav2b" onMouseLeave={productDropleave} className="dropdown-menu">
                <li><Link className="dropdown-item" href={"/productlist"}>Products List</Link></li>
                <li><Link className="dropdown-item" href={"/productcatalog"}>Products Catalog</Link></li>
                <li><hr className="dropdown-divider"/></li>
                <li><Link className="dropdown-item" href={"/productsearch"}>Product Search</Link></li>
              </ul>
            </li>
            <li className="nav-item">
              <Link onMouseEnter={productDropdownHide} className="nav-link text-white active" href={"/contactus"}><FontAwesomeIcon icon={faMapLocation}/>&nbsp;Contact Us</Link>
            </li>
          </ul>
          <ul className="navbar-nav mr-auto">
          {
            username !== '' ? (            
              <li className="nav-item dropdown">
                <Link className="nav-link dropdown-toggle text-white active" href={""} role="button" data-bs-toggle="dropdown" aria-expanded="false">
                  {
                      profilepic !== null ?
                          <Image src={profilepic} id="userpic1" fill={true} className='user ' alt={'userpicture'} />
                      : null
                  }
                  <span className='text-white'>{username}</span>
                </Link>
                <ul className="dropdown-menu">
                  <li><Link onClick={logout} className="dropdown-item" href={"/#"}>Logout</Link></li>
                  <li><Link className="dropdown-item" href={"/profile"}>Profile</Link></li>
                  <li><hr className="dropdown-divider"/></li>
                  <li><Link className="dropdown-item" href={"/#"}>Messenger</Link></li>
                </ul>
              </li>
            ) : (
            <>
            <li className="nav-item">
                <Link className="nav-link text-white active" href={"/#"} data-bs-toggle="modal" data-bs-target="#staticLogin"><FontAwesomeIcon icon={faUnlock}/>&nbsp;Login</Link>
            </li>
            <li className="nav-item">
                <Link className="nav-link text-white active" href={"/#"} data-bs-toggle="modal" data-bs-target="#staticRegister"><FontAwesomeIcon icon={faAddressCard}/>&nbsp;Register</Link>
            </li>
            </>
            )
          }
        </ul>

        </div>
      </div>
    </nav>
      <Login/>
      <Register/>

      {/* OFFCANVAS MENU */}
      <div className="offcanvas offcanvas-end" data-bs-scroll="true" id="offcanvasMenu" aria-labelledby="offcanvasWithBothOptionsLabel">
      <div className="offcanvas-header bg-primary">
        <h5 className="offcanvas-title text-white" id="offcanvasWithBothOptionsLabel">Drawer Menu</h5>
        <button type="button" className="btn-close btn-close-white" data-bs-dismiss="offcanvas" aria-label="Close"></button>
      </div>
      <div className="offcanvas-body">

        <ul className="nav flex-column">
          <li className="nav-item" data-bs-dismiss="offcanvas">
            <Link className="nav-link active" aria-current="page" href={"/aboutus"}><FontAwesomeIcon icon={faCircleQuestion}/>&nbsp;About Us</Link>
          </li>
          <li className="nav-item"><hr/></li>
          <li className="nav-item dropdown">
                <Link className="nav-link dropdown-toggle active" href={"/#"} role="button" data-bs-toggle="dropdown" aria-expanded="false">
                  <FontAwesomeIcon icon={faList}/>&nbsp;Products
                </Link>
                <ul className="dropdown-menu">
                  <li data-bs-dismiss="offcanvas"><Link className="dropdown-item" href={"/productlist"}>Products List</Link></li>
                  <li><hr className="dropdown-divider"/></li>
                  <li data-bs-dismiss="offcanvas"><Link className="dropdown-item" href={"/productcatalog"}>Products Catalog</Link></li>
                  <li><hr className="dropdown-divider"/></li>
                  <li data-bs-dismiss="offcanvas"><Link className="dropdown-item" href={"/productsearch"}>Product Search</Link></li>
                </ul>
            </li>

            <li className="nav-item"><hr/></li>
            <li className="nav-item" data-bs-dismiss="offcanvas">
              <Link className="nav-link active" aria-current="page" href={"/contactus"}><FontAwesomeIcon icon={faMapLocation}/>&nbsp;Contact Us</Link>
            </li>
            <li className="nav-item"><hr/></li>

            </ul>
            
            { 
              username !== '' ? (
                <ul className="navbar-nav mr-auto">              
                  <li className="nav-item dropdown">
                    <Link className="nav-link dropdown-toggle active" href={"/#"} role="button" data-bs-toggle="dropdown" aria-expanded="false">
                    {
                        profilepic !== null ?
                            <Image width={30} height={30} src={profilepic} id="userpic1" className='user' alt='' />
                        : null
                    }
                    {username}
                    </Link>
                    <ul className="dropdown-menu">
                      <li data-bs-dismiss="offcanvas">
                        <Link onClick={logout} className="dropdown-item" href={"/#"}>LogOut</Link>
                      </li>
                      <li className="nav-item"><hr/></li>
                      <li data-bs-dismiss="offcanvas">
                        <Link className="dropdown-item" href={"/profile"}>Profile</Link> 
                      </li>
                      <li><hr className="dropdown-divider"/></li>
                      <li data-bs-dismiss="offcanvas">
                        <Link className="dropdown-item" href={"/#"}>Messenger</Link>
                      </li>
                    </ul>
                  </li>          
                  <li className="nav-item"><hr/></li>                                        
                </ul>                    
              ) : (
                  <ul className="nav flex-column">
                    <li className="nav-item" data-bs-dismiss="offcanvas">
                      <Link className="nav-link active" href={"/#"} data-bs-toggle="modal" data-bs-target="#staticLogin"><FontAwesomeIcon icon={faUnlock}/>&nbsp;Login</Link>
                    </li>
                    <li className="nav-item"><hr/></li>
                    <li className="nav-item" data-bs-dismiss="offcanvas">
                      <Link className="nav-link active" href={"/#"} data-bs-toggle="modal" data-bs-target="#staticRegister"><FontAwesomeIcon icon={faAddressCard}/>&nbsp;Register</Link>
                    </li>            
                  </ul>
              )    
            }


      </div>
    </div>
    </div>
    );
}