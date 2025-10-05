'use client'
import { useEffect, useState } from 'react'
import axios from 'axios';
import Footer from './layout/footer';

const api = axios.create({
  baseURL: "https://localhost:7292",
  headers: {'Accept': 'application/json',
            'Content-Type': 'application/json'}
})

export type Products = {
  id: number;
  descriptions: string;
  qty: number;
  unit: string;
  sellPrice: number;
  productPicture: string;
}

export type Productdata = {
  totpage: number;
  page: number;
  products: Products[];
}


const toDecimal = (number: number) => {
  const formatter = new Intl.NumberFormat('en-US', {
    minimumFractionDigits: 2, // Ensures at least two decimal places
    maximumFractionDigits: 2, // Limits to two decimal places
  });
  // Format the number
  return formatter.format(number);
};


const Productlist = () => {
    const [page, setPage] = useState<number>(1);
    const [totpage, setTotpage] = useState<number>(0);
    const [products, setProducts] = useState<any[]>([]);
    const [message, setMessage] = useState<string>('');

    const fetchProducts = async (pg: number) => {
      setMessage('please wait...');
      await api.get<Productdata>(`/api/listproducts/${pg}`)
      .then((res) => {
        const jdata: Productdata = res.data;
        setProducts(jdata.products);
        setPage(jdata.page);
        setTotpage(jdata.totpage);
      }, (error) => {
          if (error.message === null) {
            setMessage(error.message);
          } else {
              // setMessage(error.response.data.message);
          }
              setTimeout(() => {
                setMessage('');
              }, 3000);
      });      
    }

    useEffect(() => {
      fetchProducts(page);
      setTimeout(() => {
        setMessage('');
      }, 3000);
   },[page]);

    function firstPage() {
        let pg = page;
        pg = 1;        
        fetchProducts(pg);
        return;    
      }
    
      function nextPage() {
        if (page === totpage) {
            return;
        }
        let pg = page;
        pg++;
        setPage(pg);
        fetchProducts(pg);
        return;
      }
    
      function prevPage() {
        if (page === 1) {
          return;
          }
          let pg = page;
          pg--;
          fetchProducts(pg);
          return;    
      }
    
      function lastPage() {
        let pg = page;
        pg = totpage;
        fetchProducts(pg);
        return;    
      }

    return(
    <div className="container">
            <h1 className='text-white'>Products List</h1>
            <div className='text-warning xtop'>{message}</div>
            <table className="table mt-4">
            <thead>
                <tr>
                <th scope="col">#</th>
                <th scope="col">Descriptions</th>
                <th scope="col">Qty</th>
                <th scope="col">Unit</th>
                <th scope="col">Price</th>
                </tr>
            </thead>
            <tbody>

            {products.map((item) => {
            return (
              <tr key={item['id']}>
                 <td>{item['id']}</td>
                 <td>{item['descriptions']}</td>
                 <td>{item['qty']}</td>
                 <td>{item['unit']}</td>
                 <td>&#8369;{toDecimal(item['sellPrice'])}</td>
               </tr>
              );
        })}
            </tbody>
            </table>

        <nav aria-label="Page navigation example">
        <ul className="pagination">
          <li className="page-item"><button type="button" onClick={lastPage} className="page-link" >Last</button></li>
          <li className="page-item"><button type="button" onClick={prevPage} className="page-link" >Previous</button></li>
          <li className="page-item"><button type="button" onClick={nextPage} className="page-link" >Next</button></li>
          <li className="page-item"><button type="button" onClick={firstPage} className="page-link" >First</button></li>
          <li className="page-item page-link text-danger">Page&nbsp;{page} of&nbsp;{totpage}</li>

        </ul>
      </nav>
    <Footer/>
  </div>
  )
}
export default Productlist;