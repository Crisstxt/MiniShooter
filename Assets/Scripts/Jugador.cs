using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jugador : MonoBehaviour
{
    #region Variables
    private Vector2 velRotacion;
    private new Rigidbody rigidbody;

    #endregion

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        velRotacion = new Vector2();
    }

    void FixedUpdate()
    {
        Moviemiento();
    }

    void Update()
    {
        Camara();
        Disparar();
        Salto();
    }

    private void Camara()
    {

        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * MiniShooter.instance.GetSensibilidadRaton ;

        velRotacion.x = Mathf.Lerp(velRotacion.x, velRotacion.x + mouseDelta.x, MiniShooter.instance.GetSuavizado() * Time.deltaTime);
        velRotacion.y = Mathf.Lerp(velRotacion.y, velRotacion.y + mouseDelta.y, MiniShooter.instance.GetSuavizado() * Time.deltaTime);

        velRotacion.y = Mathf.Clamp(velRotacion.y, -MiniShooter.instance.GetLimiteRotacionVertical, MiniShooter.instance.GetLimiteRotacionVertical);

        Camera.main.transform.localRotation = Quaternion.AngleAxis(-velRotacion.y, Vector3.right);
        transform.localRotation = Quaternion.AngleAxis(velRotacion.x, Vector3.up);

        if (MiniShooter.instance.GetPosicionesCamara.Length > 0 && Input.GetKeyDown(KeyCode.C))
            Camera.main.transform.localPosition = MiniShooter.instance.CambiarPosicionCamara();               
    }

    private void Moviemiento()
    {
        float velocidadX = Input.GetAxis("Horizontal") * MiniShooter.instance.GetVelocidadJugador;
        float veloccidadZ = Input.GetAxis("Vertical") * MiniShooter.instance.GetVelocidadJugador;
        rigidbody.velocity = transform.rotation * new Vector3(velocidadX, rigidbody.velocity.y, veloccidadZ);

        if (Input.GetKey(KeyCode.LeftShift)) MiniShooter.instance.Correr();
        else MiniShooter.instance.Caminar();
    }


    private void Salto()
    {
        if (Input.GetKeyDown(KeyCode.Space)) rigidbody.AddForce(Vector3.up * MiniShooter.instance.GetFuerzaSalto(), ForceMode.Impulse);
    }


    private void Disparar()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray rayo = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(rayo, out RaycastHit hit)) 
            {
                hit.collider.gameObject.GetComponent<Enemigo>()?.Destruir();
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other != null)
        {
            if (other.CompareTag("Enemigo")) MiniShooter.instance.FinPartida();

            if (other.CompareTag("Moneda")) other.GetComponent<Moneda>()?.RecolectarMoneda();
        }
    }
}
