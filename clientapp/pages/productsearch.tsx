import React, { useState } from 'react'
import axios from 'axios';
import Footer from './layout/footer';

const api = axios.create({
  baseURL: "https://localhost:7292",
  headers: {'Accept': 'application/json',
            'Content-Type': 'application/json'}
})

const formatNumberWithCommaDecimal = (number: any) => {
  const formatter = new Intl.NumberFormat('en-US', {
    minimumFractionDigits: 2, // Ensures at least two decimal places
    maximumFractionDigits: 2, // Limits to two decimal places
  });
  // Format the number
  return formatter.format(number);
};

interface Productdata {
  totpage: string,
  page: string,
  products: Products
}

interface Products {
  id: number,
  descriptions: string,  
  qty: number,
  unit: string,
  sellPrice: number,
  productPicture: string
}
const Productsearch = (props: any) => {
    const [prodsearch, setProdsearch] = useState<Products[]>([]);
    const [message, setMessage] = useState<string>('');
    let [searchkey, setSearchkey] = useState<string>('');

    const getProdsearch = async (event: any) => {
        event.preventDefault();
        setMessage("please wait .");
        const data = JSON.stringify({ search: searchkey});

        api.post<Productdata>("/api/searchproducts",data)
        .then((res) => {
          const data: Productdata = res.data;
            setProdsearch(data.products);
        }, (error: any) => {
            // setErrors(error.message);
            console.log(error.message);
            return;
        });  
        setMessage('');
    }
     
  return (
    <div className="container mb-4">
        <h2>Prouct Search</h2>

        <form className="row g-3" onSubmit={getProdsearch} autoComplete='off'>
            <div className="col-auto">
              <input type="text" required className="form-control-sm" value={searchkey} onChange={e => setSearchkey(e.target.value)} placeholder="enter Product keyword"/>
            </div>
            <div className="col-auto">
              <button type="submit" className="btn btn-primary btn-sm mb-3">search</button>
            </div>
            <div className='searcMsg'>{message}</div>

        </form>
        <div className="container">
          <div className="card-group">
        {prodsearch.map((item) => {
                return (
                <div className='col-md-4'>
                <div key={item.id} className="card mx-3 mt-3">
                    <img src={item['productPicture']} className="card-img-top product-size" alt=""/>
                    <div className="card-body">
                      <h5 className="card-title">Descriptions</h5>
                      <p className="card-text desc-h">{item['descriptions']}</p>
                    </div>
                    <div className="card-footer">
                      <p className="card-text text-danger"><span className="text-dark">PRICE :</span>&nbsp;<strong>&#8369;{item['sellPrice']}</strong></p>
                    </div>  
                </div>
                </div>
          );    
        })}
          </div>          
        </div>
        <Footer/>
    </div>
  )
}

export default Productsearch;