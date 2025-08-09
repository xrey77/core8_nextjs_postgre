import React, { useState } from 'react'
import Image from 'next/image'

const Productsearch = (props: any) => {
    const [prodsearch, setProdsearch] = useState([]);
    const [message, setMessage] = useState("");
    let [searchkey, setSearchkey] = useState("");
    
    const getProdsearch = async (event: any) => {
        event.preventDefault();                 
        setMessage("please wait .");
        let response = await fetch(`/api/product/search?search=${searchkey}`);
        let data = await response.json();     
        if (data.statuscode === 200) {
          setProdsearch(data.product);
        } else {
          setMessage(data.message);
        }
        window.setTimeout(() => {
          setMessage("");
        }, 3000);  

    }
     
  return (
    <div className="container mb-4">
        <h2>Search Product</h2>

        <form className="row g-3" onSubmit={getProdsearch} autoComplete='off'>
            <div className="col-auto">
              <input type="text" required className="form-control-sm" value={searchkey} onChange={e => setSearchkey(e.target.value)} placeholder="enter Product keyword"/>
            </div>
            <div className="col-auto">
              <button type="submit" className="btn btn-primary btn-sm mb-3">search</button>
            </div>
            <div className='searcMsg'>{message}</div>

        </form>
        
        <div className="card-group mb-4">
        {prodsearch.map((item) => {
                return (
                  <div key={item.id} className="card">
                    <Image src={item['prod_pic']} className="card-img-top product-size" alt="..." width={250} height={250}/>
                    <div className="card-body">
                      <h5 className="card-title">Descriptions</h5>
                      <p className="card-text">{item['descriptions']}</p>
                    </div>
                    <div className="card-footer">
                      <p className="card-text text-danger"><span className="text-dark">PRICE :</span>&nbsp;<strong>
                        &#8369;{item['sell_price']}</strong></p>
                    </div>   
                  </div>                  
                );
        })}
        </div>    
      
    </div>
  )
}

export default Productsearch;