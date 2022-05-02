extern crate libc;
use std::io::Write;
use libc::{c_char, size_t};

#[no_mangle]
pub unsafe extern "C" fn stdout_int_ln(msg: size_t) {
    println!("{}", msg);
}

#[no_mangle]
pub unsafe extern "C" fn stdout(msg: *mut c_char) {
    let msg_str: &str = match std::ffi::CStr::from_ptr(msg).to_str() {
        Ok(s) => s,
        Err(_) => &"Foo", // Panicing is probably more appropriate...
    };

    print!("{}", msg_str);
    std::io::stdout().flush().unwrap();
}

#[no_mangle]
pub unsafe extern "C" fn stdout_ln(msg: *mut c_char) {
    let msg_str: &str = match std::ffi::CStr::from_ptr(msg).to_str() {
        Ok(s) => s,
        Err(_) => &"Foo", // Panicing is probably more appropriate...
    };

    println!("{}", msg_str);
}